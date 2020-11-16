using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Spawning : MonoBehaviour
{
    // How much time between each spawn initially set in the editor
    public float startTimer;

    // How much time between each spawn
    float timeGap;

    // Used to radomly choose which hazard will spawn
    int rand;

    // The cactus prefab goes here
    public GameObject cactus;
    public GameObject doubleCactus;
    public GameObject bird;

    // Start is called before the first frame update
    void Start()
    {
        // Makes it easier to make use of the original time set manually by copying it over to another value
        timeGap = startTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Will keep counting down if still above zero
        if (timeGap > 0)
        {
            timeGap -= Time.deltaTime;
        }

        // Will reset the timer and spawn a hazard if under zero
        else if (timeGap <= 0)
        {
            // Randomizes a 0 or a 1 or a 2 value
            rand = Random.Range(0, 3);

            // Spawn a cactus if 0
            if (rand == 0)
            {
                //randomize a 0 or 1
                rand = Random.Range(0, 2);

                //spawn cactus if 0
                if (rand == 0)
                    Instantiate(cactus, new Vector3(9.5f, 0.82f, 0), Quaternion.identity);

                //spawn cactus if 1
                if (rand == 1)
                    Instantiate(doubleCactus, new Vector3(9.5f, 0.82f, 0), Quaternion.identity);
            }
            // Spawn a bird if 1
            else if (rand == 1)
            {
                // Randomize a 0 or 1 value
                rand = Random.Range(0, 2);

                // Spawn the bird low if 0
                if (rand == 0)
                    Instantiate(bird, new Vector3(9.5f, 1.5f, 0), Quaternion.identity);

                // Spawn the bird high if 1
                if (rand == 1)
                    Instantiate(bird, new Vector3(9.5f, 3.5f, 0), Quaternion.identity);
            }
            //spawn bird cactus combo
            else if (rand == 2)
            {
                // Randomize a 0 or 1 value
                rand = Random.Range(0, 2);

                // Spawn high bird and single cactus
                if (rand == 0)
                {
                    Instantiate(cactus, new Vector3(9.5f, 0.82f, 0), Quaternion.identity);
                    Instantiate(bird, new Vector3(9.5f, 4.0f, 0), Quaternion.identity);
                }

                // Spawn low bird and couple cactus
                if (rand == 1)
                {
                    Instantiate(doubleCactus, new Vector3(9.5f, 0.82f, 0), Quaternion.identity);
                    Instantiate(bird, new Vector3(9.75f, 1.8f, 0), Quaternion.identity);
                }
            }
            // Reset the timer to the original value
            timeGap = startTimer;
        }
    }
}
