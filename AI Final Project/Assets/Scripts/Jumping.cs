using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : MonoBehaviour
{

    Rigidbody2D rigidbody2D;

    [SerializeField]
    float jumpVelocity = 7.0f;
    [SerializeField]
    float fallMultiplier = 2.5f;
    [SerializeField]
    public float lowJumpMultiplier = 2.5f;

    bool jump = false;

    public bool GetJump()
    {
        return jump;
    }
    public void SetJump(bool b)
    {
        jump = b;
    }

    // Start is called before the first frame update
    void Start()
    {
        //sets the rigidbody to find the rigidbody attached to the dino
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // sets a bool to true which is the request for the dino to jump
        if (Input.GetKeyDown(KeyCode.Space) && rigidbody2D.velocity.y == 0.0f)//makes sure it is on the groud
            jump = true;    
    }

    private void FixedUpdate()//physics update
    {
        if (jump)//where the jump happens
        {
            rigidbody2D.velocity += Vector2.up * jumpVelocity;
            jump = false;
        }

        if (rigidbody2D.velocity.y < 0)//where the fast fall happens
        {
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody2D.velocity.y > 0 && !Input.GetKey(KeyCode.Space))//where the short jump happens
        {
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
