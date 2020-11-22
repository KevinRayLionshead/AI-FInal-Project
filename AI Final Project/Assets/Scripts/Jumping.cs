using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : MonoBehaviour
{

    Rigidbody2D rigidbody2D;

    [SerializeField]
    float jumpVelocity = 80.0f;
    [SerializeField]
    float fallMultiplier = 2.5f;
    [SerializeField]
    public float lowJumpMultiplier = 2.5f;

    bool jump = false;
    bool jumpKey = false;
    public bool highJump = false;
    bool highJumpKey = false;

    public bool GetJump()
    {
        return jumpKey;
    }
    public void SetJump(bool b)
    {
        jumpKey = b;
    }
    public void SetHighJump(bool b)
    {
        highJumpKey = b;
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
        if ((Input.GetKeyDown(KeyCode.Space) || jumpKey) && gameObject.transform.position.y < 1.1f)//makes sure it is on the groud
            jump = true;

        else if ((highJumpKey) && gameObject.transform.position.y < 1.1f)//makes sure it is on the groud
            highJump = true;
    }

    private void FixedUpdate()//physics update
    {
        if (jump)//where the jump happens
        {
            rigidbody2D.velocity += Vector2.up * 5;
            jump = false;
        }
        else if (highJump)
        {
            rigidbody2D.velocity += Vector2.up * 5;
            highJump = false;
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
