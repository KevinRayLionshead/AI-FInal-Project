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

    // How much time between each offset spawn
    float timeGapOff;

    // Set the offset between the two spawners
    public int offset;

    // Used to radomly choose which hazard will spawn
    int rand;

    // The cactus prefab goes here
    public GameObject cactus;
    public GameObject bird;

    // Start is called before the first frame update
    void Start()
    {
        // Makes it easier to make use of the original time set manually by copying it over to another value
        timeGap = startTimer;
        timeGapOff = startTimer * offset;
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
            // Randomizes a 0 or a 1 value
            rand = Random.Range(0, 2);

            // Spawn a cactus if 0
            if (rand == 0)
            {
                Instantiate(cactus, new Vector3(9.5f, 0.9f, 0), Quaternion.identity);
            }
            // Spawn a bird if 1
            if (rand == 1)
            {
                // Randomize a 0 or 1 value
                rand = Random.Range(0, 2);

                // Spawn the bird low if 0
                if (rand == 0)
                    Instantiate(bird, new Vector3(9.5f, 2.5f, 0), Quaternion.identity);

                // Spawn the bird high if 1
                if (rand == 1)
                    Instantiate(bird, new Vector3(9.5f, 3.5f, 0), Quaternion.identity);
            }
            // Reset the timer to the original value
            timeGap = startTimer;
        }

        // Will keep counting down if still above zero
        if (timeGapOff > 0)
        {
            timeGapOff -= Time.deltaTime;
        }

        // Will reset the timer and spawn a hazard if under zero
        else if (timeGapOff <= 0)
        {
            // Randomizes a 0 or a 1 value
            rand = Random.Range(0, 6);

            if (rand == 3 || rand == 4)
            {
                Instantiate(cactus, new Vector3(9.5f, 0.9f, 0), Quaternion.identity);
            }

            // Spawn a bird if 1
            if (rand == 5)
            {
                // Randomize a 0 or 1 value
                rand = Random.Range(0, 2);

                // Spawn the bird low if 0
                if (rand == 0)
                    Instantiate(bird, new Vector3(9.5f, 2.5f, 0), Quaternion.identity);

                // Spawn the bird high if 1
                if (rand == 1)
                    Instantiate(bird, new Vector3(9.5f, 3.5f, 0), Quaternion.identity);
            }

            timeGapOff = startTimer / offset;
        }
    }
}
