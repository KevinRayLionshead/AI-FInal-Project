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

    int numOfSteps = 1500;//these are the steps the AI goes through for each iteration
    int counter = 0;//the counter for the steps

    float gamma = 0.9f;

    public enum Action { Idle, Jump, Duck, HighJump };//the 4 actions the AI can perform

    public struct State//struct holding the different states
    {
        //size = 2^number of bools
        //bool running;
        public bool highJumping;//high jump state
        public bool jumping;//regular jump state
        public bool ducking;//ducking state
        public bool dead;// dead state

        public bool objectInfront;//a state for when a cactus is in front of the AI
        public bool wideObject;//a state where there is a double cactus in front of the AI
    };
    int numOfStates = (int)Mathf.Pow(2, 6.0f);// this number is the number of Unique states that can take place
    

    Matrix<float> pMatrix;//probability matrix
    Matrix<float> identity;//identity matrix
    

    State initialState;//previous state to check against
    State currentState;//current state that AI is in

    List<State> stateList;

    Vector<float> states;
    Vector<float> rewards;
    Vector<float> cumulativeRewards;
    
    public struct Samples//creates a sample struct
    {
        public State curState;// the current state
        public Action action;//the current action
        public State nextState;//the next state
        public int freq;//frequency

        public override bool Equals(object b)// operator for allowing the equals of 2 samples
        {
            if (!(b is Samples))
            { return false; }

            Samples otherSample = (Samples)b;

            return curState.Equals(otherSample.curState) && action == otherSample.action && nextState.Equals(otherSample.nextState);
        }

        public static bool operator ==(Samples a, Samples b) //operator for comparing samples
        {
            return a.curState.Equals(b.curState) && a.action == b.action && a.nextState.Equals(b.nextState);
        }
        public static bool operator !=(Samples a, Samples b)//operator for also comapring samples for inequality
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
        jumping = gameObject.GetComponent<Jumping>();//adds a reference to the jumping script
        crouch = gameObject.GetComponent<Crouch>();//adds a reference to the crouching script

        pMatrix = Matrix<float>.Build.Dense(numOfStates, numOfStates);//creates a probability matrix the size of the number of unique states
        identity = Matrix<float>.Build.DenseIdentity(numOfStates);//creates a matrix the same size but as an identity matrix
        Debug.Log(identity);

        stateList = new List<State>();

        samples = new List<Samples>();

        states = Vector<float>.Build.Dense(numOfStates);//creates an array/vector for storing the number of single states
        rewards = Vector<float>.Build.Dense(numOfStates);//creates a vector that holds the reward for each state
        cumulativeRewards = Vector<float>.Build.Dense(numOfStates);//holds the sum of all rewards each state received


        //for (int i = 0; i < numOfStates; i++)
        //{
        //    states[i] = i;
        //}

        //sets all states to false for the start
        initialState.highJumping = false;
        initialState.jumping = false;
        initialState.ducking = false;
        initialState.dead = false;
        initialState.objectInfront = false;
        initialState.wideObject = false;

        currentState = initialState;//sets current state to blank

        State tempState = new State();//holds all the unique states temporarily

        for (int a = 0; a < 2; a++)//these for loops combine each state to get all unique possible states
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

        for (int i = 0; i < numOfStates; i++)// This for loop sets the reward/punish amount for each of the single states
        {
            //if (!stateList[i].dead && !stateList[i].objectInfront && !stateList[i].wideObject)
            //    rewards[i] += 3;
            //else if (stateList[i].objectInfront && !stateList[i].jumping)
            //    rewards[i] += -2;
            //else if (stateList[i].objectInfront && !stateList[i].ducking)
            //    rewards[i] += 1;
            //else if (stateList[i].dead)
            //    rewards[i] += -10;

            if (stateList[i].dead)//sets punish for death
                rewards[i] += -30;
            else
                rewards[i] += 1;
            if (stateList[i].ducking)//sets reward for ducking
                rewards[i] += 1;
            if (stateList[i].highJumping)//sets reward for highjump
                rewards[i] += 5;
            if (stateList[i].jumping)//sets reward for regular jump
                rewards[i] += 10;
            if (stateList[i].objectInfront)//sets punish for when a cactus is in front of AI
                rewards[i] += -2;
            else
                rewards[i] += 1;
            if (stateList[i].wideObject)//sets punish for when a double cactus is in from of AI
                rewards[i] += -2;
            else
                rewards[i] += 1;

            if (stateList[i].jumping && stateList[i].objectInfront)// sets reward for jumping when in front of cactus
                rewards[i] += 10;
            if (stateList[i].jumping && stateList[i].highJumping && stateList[i].wideObject)// sets reward for jumping when in front of double cactus
                rewards[i] += 10;

            if (!stateList[i].jumping && stateList[i].objectInfront)// sets punish for not jumping when in front of cactus
                rewards[i] += -20;
            if (!stateList[i].jumping && !stateList[i].highJumping && stateList[i].wideObject)// sets punish for not high jumping when in front of double cactus
                rewards[i] += -20;

            if (!stateList[i].ducking && !stateList[i].dead && !stateList[i].highJumping
                 && !stateList[i].jumping && !stateList[i].objectInfront && !stateList[i].wideObject)
                rewards[i] += 25;// if surviving it will get rewarded

        }

    }

    void CreateProbMat()//creates a probability matrix
    {
        for (int i = 0; i < numOfStates; i++)
        {
            float rowSum = 0;
            for (int j = 0; j < numOfStates; j++)
            {
                rowSum += pMatrix[i, j];//keeps tab of the frequency each state has been triggered
            }
            for (int j = 0; j < numOfStates; j++)
            {
                if (rowSum == 0)
                {
                    pMatrix[i, j] = 0;
                }
                else
                {
                    pMatrix[i, j] /= rowSum;//divides each cell by the row sum to get a 0-1 value
                }// all the values combined will get you a total of 1 making it a probability matrix
                
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        State nextState;

        Action nextAction = new Action();

        if(counter < numOfSteps)//will go into this if until each iteration is done
        {
            
            int i = stateList.IndexOf(currentState);//sets the reward for the current state
            cumulativeRewards[i] += rewards[i];

            bool containsCurState = false;

            for (int j = 0; j < samples.Count; j++)//figures out which state is the current state
            {
                if (samples[j].curState.Equals(currentState))
                {
                    containsCurState = true;
                    break;
                }
            }

            if(samples.Count < 1 || !containsCurState)//if there is no sample to follow a random action will be chosen
            {
                nextAction = (Action)Random.Range(0, 4);
                
            }
            else
            {
                int curStateFreq = 0;
                List<ActionDec> actionsList = new List<ActionDec>();//create a list of actions

                for (int j = 0; j < samples.Count; j++)
                {
                    if (samples[j].curState.Equals(currentState))//checks to see if the samples current state is the same as this current state
                    {
                        curStateFreq += samples[j].freq;//gets the frequency of all the samples in the subset

                        ActionDec temp;
                        temp.action = samples[j].action;
                        temp.eval = 0;
                        actionsList.Add(temp);//adds to the list fo actions if the current state is the same
                    }
                }


                for (int l = 0; l < actionsList.Count; l++)//breaks down the samples into subsamples
                {
                    for (int j = 0; j < samples.Count; j++)
                    {
                        if (actionsList[l].action == samples[j].action && samples[j].curState.Equals(currentState))
                        {
                            ActionDec temp1;
                            temp1 = actionsList[l];

                            //adjusts the evaluation of each state for the best move to get the best reward
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
                
                for (int j = 0; j < actionsList.Count; j++)// this for loop finds the action with the greatest average
                {
                    if (actionsList[j].eval > maxEval)
                        nextAction = actionsList[j].action;
                }
                

            }

            nextState = currentState;
            switch (nextAction)//performs the action with the highest average
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

            pMatrix[i, k] += 1; //This keeps track of this current states next state

            

            
            Samples tempSample = new Samples();//creates a temp sample for the below if statement

            tempSample.curState = currentState;
            tempSample.action = nextAction;
            tempSample.freq = 1;

            if (samples.Contains(tempSample))//this adds up the frequency of the samples for probability calculations
            {
                tempSample.freq += samples[samples.IndexOf(tempSample)].freq;
                samples[samples.IndexOf(tempSample)] = tempSample;

            }
            else//if the sample has not been added yet then it will add it
            {
                samples.Add(tempSample);
            }

           

            currentState = nextState;
            //sets the state for the current state
            jumping.SetJump(currentState.jumping);
            jumping.SetHighJump(currentState.highJumping);
            crouch.SetCrouch(currentState.ducking);


        }
        else//this is for when the next iteration is coming
        {
            Debug.Log("Finished "+ numOfSteps);
            CreateProbMat();//calls the create probability matrix

            // this states vector takes in probability matrix * the gamma(or discount term) which you then take the inverse matrix of
            // and multiply it with the cumulative rewards vector to iterate on the states vector whcih represents the value estimate for all states
            states = (identity - (gamma * pMatrix)).Inverse() * cumulativeRewards;

            counter = 0;//resest the counter
            pMatrix = Matrix<float>.Build.Dense(numOfStates, numOfStates);//resets probability matrix to all zeroes



            samples.Clear();//clears the samples to start from scratch

        }


        counter++;//increase the count for the steps
    }
}
