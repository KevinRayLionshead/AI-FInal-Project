using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;



public class AI : MonoBehaviour
{
    Jumping jumping;
    Crouch crouch;
    public GameObject objectCollision;

    int numOfSteps = 1500;
    int counter = 0;

    float gamma = 0.9f;

    public enum Action { Idle, Jump, Duck, HighJump };

    public struct State
    {
        //size = 2^number of bools
        //bool running;
        public bool highJumping;
        public bool jumping;
        public bool ducking;
        public bool dead;

        public bool objectInfront;
        public bool wideObject;
    };
    int numOfStates = (int)Mathf.Pow(2, 6.0f);
    

    Matrix<float> pMatrix;
    Matrix<float> identity;
    

    State initialState;
    State currentState;

    List<State> stateList;

    Vector<float> states;
    Vector<float> rewards;
    Vector<float> cumulativeRewards;
    
    public struct Samples
    {
        public State curState;
        public Action action;
        public State nextState;
        public int freq;

        public bool Equals(Samples b)
        {
            return curState.Equals(b.curState) && action == b.action && nextState.Equals(b.nextState);
        }

        public static bool operator ==(Samples a, Samples b)
        {
            return a.curState.Equals(b.curState) && a.action == b.action && a.nextState.Equals(b.nextState);
        }
        public static bool operator !=(Samples a, Samples b)
        {
            return !(a.curState.Equals(b.curState) && a.action == b.action && a.nextState.Equals(b.nextState));
        }
    }

    
    

    List<Samples> samples;

    struct ActionDec
    {
        public Action action;
        public float eval;
    }


    //prob matrix nxn

    //v = colum vector (states) num at index is the value assosiated with state
    //I is identity matrix mxm
    //Gamma set to 1 (0-1) I set it 0 cancels prob matrix (means greedy)
    // P prob matrix all states in rows and colums
    //r is colum vector (size num of states) keeps track of current reward for all states







    // Start is called before the first frame update
    void Start()
    {
        jumping = gameObject.GetComponent<Jumping>();
        crouch = gameObject.GetComponent<Crouch>();

        pMatrix = Matrix<float>.Build.Dense(numOfStates, numOfStates);
        identity = Matrix<float>.Build.DenseIdentity(numOfStates);
        Debug.Log(identity);

        stateList = new List<State>();

        samples = new List<Samples>();

        states = Vector<float>.Build.Dense(numOfStates);
        rewards = Vector<float>.Build.Dense(numOfStates);
        cumulativeRewards = Vector<float>.Build.Dense(numOfStates);


        //for (int i = 0; i < numOfStates; i++)
        //{
        //    states[i] = i;
        //}


        initialState.highJumping = false;
        initialState.jumping = false;
        initialState.ducking = false;
        initialState.dead = false;
        initialState.objectInfront = false;
        initialState.wideObject = false;

        currentState = initialState;

        State tempState = new State();

        for (int a = 0; a < 2; a++)
        {
            tempState.highJumping = (a == 1);
            for (int b = 0; b < 2; b++)
            {
                tempState.jumping = (b == 1);
                for (int c = 0; c < 2; c++)
                {
                    tempState.ducking = (c == 1);
                    for (int d = 0; d < 2; d++)
                    {
                        tempState.dead = (d == 1);
                        for (int e = 0; e < 2; e++)
                        {
                            tempState.objectInfront = (e == 1);
                            for (int f = 0; f < 2; f++)
                            {
                                tempState.wideObject = (f == 1);
                                stateList.Add(tempState);
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < numOfStates; i++)
        {
            //if (!stateList[i].dead && !stateList[i].objectInfront && !stateList[i].wideObject)
            //    rewards[i] += 3;
            //else if (stateList[i].objectInfront && !stateList[i].jumping)
            //    rewards[i] += -2;
            //else if (stateList[i].objectInfront && !stateList[i].ducking)
            //    rewards[i] += 1;
            //else if (stateList[i].dead)
            //    rewards[i] += -10;

            if (stateList[i].dead)
                rewards[i] += -30;
            else
                rewards[i] += 1;
            if (stateList[i].ducking)
                rewards[i] += 1;
            if (stateList[i].highJumping)
                rewards[i] += 5;
            if (stateList[i].jumping)
                rewards[i] += 10;
            if (stateList[i].objectInfront)
                rewards[i] += -2;
            else
                rewards[i] += 1;
            if (stateList[i].wideObject)
                rewards[i] += -2;
            else
                rewards[i] += 1;

            if (stateList[i].jumping && stateList[i].objectInfront)
                rewards[i] += 10;
            if (stateList[i].jumping && stateList[i].highJumping && stateList[i].wideObject)
                rewards[i] += 10;

            if (!stateList[i].jumping && stateList[i].objectInfront)
                rewards[i] += -20;
            if (!stateList[i].jumping && !stateList[i].highJumping && stateList[i].wideObject)
                rewards[i] += -20;

            if (!stateList[i].ducking && !stateList[i].dead && !stateList[i].highJumping
                 && !stateList[i].jumping && !stateList[i].objectInfront && !stateList[i].wideObject)
                rewards[i] += 25;

        }

    }

    void CreateProbMat()
    {
        for (int i = 0; i < numOfStates; i++)
        {
            float rowSum = 0;
            for (int j = 0; j < numOfStates; j++)
            {
                rowSum += pMatrix[i, j];
            }
            for (int j = 0; j < numOfStates; j++)
            {
                if (rowSum == 0)
                {
                    pMatrix[i, j] = 0;
                }
                else
                {
                    pMatrix[i, j] /= rowSum;
                }
                
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        State nextState;

        Action nextAction = new Action();

        if(counter < numOfSteps)
        {
            
            int i = stateList.IndexOf(currentState);
            cumulativeRewards[i] += rewards[i];

            bool containsCurState = false;

            for (int j = 0; j < samples.Count; j++)
            {
                if (samples[j].curState.Equals(currentState))
                {
                    containsCurState = true;
                    break;
                }
            }

            if(samples.Count < 1 || !containsCurState)
            {
                nextAction = (Action)Random.Range(0, 4);
                
            }
            else
            {
                int curStateFreq = 0;
                List<ActionDec> actionsList = new List<ActionDec>();

                for (int j = 0; j < samples.Count; j++)
                {
                    if (samples[j].curState.Equals(currentState))
                    {
                        curStateFreq += samples[j].freq;

                        ActionDec temp;
                        temp.action = samples[j].action;
                        temp.eval = 0;
                        actionsList.Add(temp);
                    }
                }


                for (int l = 0; l < actionsList.Count; l++)
                {
                    for (int j = 0; j < samples.Count; j++)
                    {
                        if (actionsList[l].action == samples[j].action && samples[j].curState.Equals(currentState))
                        {
                            ActionDec temp1;
                            temp1 = actionsList[l];
                            
                            temp1.eval += states[stateList.IndexOf(samples[j].nextState)] * samples[j].freq;
                            actionsList[l] = temp1;
                        }
                    }
                    ActionDec temp;
                    temp = actionsList[l];
                    temp.eval /= curStateFreq;
                    actionsList[l] = temp;
                }

                float maxEval = 0;
                
                for (int j = 0; j < actionsList.Count; j++)
                {
                    if (actionsList[j].eval > maxEval)
                        nextAction = actionsList[j].action;
                }
                

            }

            nextState = currentState;
            switch (nextAction)
            {
                case Action.Idle:
                    nextState.ducking = false;
                    nextState.highJumping = false;
                    nextState.jumping = false;
                    break;
                case Action.Duck:
                    nextState.ducking = true;
                    nextState.highJumping = false;
                    nextState.jumping = false;
                    break;
                case Action.HighJump:
                    nextState.ducking = false;
                    nextState.highJumping = true;
                    nextState.jumping = false;
                    break;
                case Action.Jump:
                    nextState.ducking = false;
                    nextState.highJumping = false;
                    nextState.jumping = true;
                    break;

            }

            //Getter functions here for obj in front, wide obj and dead.

            nextState.objectInfront = objectCollision.GetComponent<ObjectCollision>().objectInfront;
            nextState.wideObject = objectCollision.GetComponent<ObjectCollision>().wideObject;

            nextState.dead = crouch.dead;






            int k = stateList.IndexOf(nextState);

            pMatrix[i, k] += 1;

            

            
            Samples tempSample = new Samples();

            tempSample.curState = currentState;
            tempSample.action = nextAction;
            tempSample.freq = 1;

            if (samples.Contains(tempSample))
            {
                tempSample.freq = samples[samples.IndexOf(tempSample)].freq;
                samples[samples.IndexOf(tempSample)] = tempSample;

            }
            else
            {
                samples.Add(tempSample);
            }

           

            currentState = nextState;

            jumping.SetJump(currentState.jumping);
            jumping.SetHighJump(currentState.highJumping);
            crouch.SetCrouch(currentState.ducking);


        }
        else
        {
            Debug.Log("Finished "+ numOfSteps);
            CreateProbMat();

            states = (identity - (gamma * pMatrix)).Inverse() * cumulativeRewards;

            counter = 0;
            pMatrix = Matrix<float>.Build.Dense(numOfStates, numOfStates);



            samples.Clear();

        }


        counter++;
    }
}
