using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScroll : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 144;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * (9 * Time.deltaTime));

        //if (transform.position.x < -9.5)
        //{

        //    Destroy(gameObject);
        //}
    }
}
