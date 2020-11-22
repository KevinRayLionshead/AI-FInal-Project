using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    public bool objectInfront;// bool that represents the small cactus
    public bool wideObject;//bool that represents the double cactus

    public GameObject dino;//the AI player

    private void OnTriggerEnter2D(Collider2D collision)
    {

        
        if (collision.tag == "enemy" && gameObject.tag == "InFrontCollider")//if the collision is with a cactus it will set to true
        {
            objectInfront = true;
            Debug.Log("Hit Enemy");
        }
        else if (collision.tag == "Wide Enemy" && gameObject.tag == "InFrontCollider")//if collision with a double cactus will set ot true
        {
            wideObject = true;
            Debug.Log("Hit Wide Enemy");
        }
    }



    private void OnTriggerExit2D(Collider2D collision)//when a collision is done it will set the bools to false
    {
        objectInfront = false;
        wideObject = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        objectInfront = false;//sets to false on start
        wideObject = false;//sets to false on start
    }

    // Update is called once per frame
    void Update()
    {   //sets the colliders position to be the same has the dino's
        GetComponent<BoxCollider2D>().transform.position = dino.transform.position;
    }
}
