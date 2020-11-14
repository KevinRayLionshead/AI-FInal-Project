using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    [SerializeField]
    BoxCollider2D normalBox;
    [SerializeField]
    BoxCollider2D crouchBox;

    Vector2 normalSize;
    Vector2 crouchSize;

    Vector2 normalPos;
    Vector2 crouchPos;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        normalBox.enabled = true;
        crouchBox.enabled = false;

        normalSize = new Vector2(transform.localScale.x, transform.localScale.y);
        crouchSize = new Vector2(normalSize.x, normalSize.y * 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C) && rigidbody2D.velocity.y == 0.0f)
        {
            normalBox.enabled = false;
            crouchBox.enabled = true;
            //transform.localScale = crouchSize;
            //transform.position = new Vector2(transform.position.x, transform.position.y + (transform.position.y * 0.5f)); 
        }
        else if (!Input.GetKey(KeyCode.C) || rigidbody2D.velocity.y != 0.0f)
        {
            normalBox.enabled = true;
            crouchBox.enabled = false;
            //transform.localScale = normalSize;
            //transform.position = new Vector2(transform.position.x, transform.position.y - (transform.position.y * 2.0f));
        }
    }
}
