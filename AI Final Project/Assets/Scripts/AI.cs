using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;



public class AI : MonoBehaviour
{
    int numOfSteps = 6000;
    int counter = 0;

    float gamma = 1;

    enum Action { Idle, Jump, HighJump, Duck};

    struct State
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
    
    struct Samples
    {
        public List<State> curState;
        public List<Action> action;
        public List<State> nextState;
        public List<int> freq;
    }

    Samples samples;

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
        pMatrix = Matrix<float>.Build.Dense(numOfStates, numOfStates);
        identity = Matrix<float>.Build.DenseIdentity(numOfStates);
        Debug.Log(identity);

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
            if (!stateList[i].dead && !stateList[i].objectInfront)
                rewards[i] = 1;
            else if (stateList[i].dead)
                rewards[i] = -1;
            else
                rewards[i] = 0;
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
                pMatrix[i, j] /= rowSum;
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

            if(samples.curState.Count < 1 || !samples.curState.Contains(currentState))
            {
                nextAction = (Action)Random.Range(0, 3);
            }
            else
            {
                int curStateFreq = 0;
                List<ActionDec> actionsList = new List<ActionDec>();

                for (int j = 0; j < samples.curState.Count; j++)
                {
                    if (samples.curState[j].Equals(currentState))
                    {
                        curStateFreq += samples.freq[j];

                        ActionDec temp;
                        temp.action = samples.action[j];
                        temp.eval = 0;
                        actionsList.Add(temp);
                    }
                }


                for (int l = 0; l < actionsList.Count; l++)
                {
                    for (int j = 0; j < samples.action.Count; j++)
                    {
                        if (actionsList[l].action == samples.action[j] && samples.curState.Equals(currentState))
                        {
                            ActionDec temp1;
                            temp1 = actionsList[l];
                            
                            temp1.eval += states[stateList.IndexOf(samples.nextState[j])] * samples.freq[j];
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

                int k = stateList.IndexOf(nextState);

                pMatrix[i, k] += 1;

                currentState = nextState;

            }

        }
        else
        {
            CreateProbMat();

            states = (identity - (gamma * pMatrix)).Inverse() * cumulativeRewards;

        }


        counter++;
    }
}
