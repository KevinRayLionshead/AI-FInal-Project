using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    public bool objectInfront;
    public bool wideObject;

    public GameObject dino;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        
        if (collision.tag == "enemy" && gameObject.tag == "InFrontCollider")
        {
            objectInfront = true;
            Debug.Log("Hit Enemy");
        }
        else if (collision.tag == "Wide Enemy" && gameObject.tag == "InFrontCollider")
        {
            wideObject = true;
            Debug.Log("Hit Wide Enemy");
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        objectInfront = false;
        wideObject = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        objectInfront = false;
        wideObject = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider2D>().transform.position = dino.transform.position;
    }
}
