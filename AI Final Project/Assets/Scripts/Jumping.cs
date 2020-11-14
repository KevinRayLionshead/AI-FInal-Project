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

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space) && rigidbody2D.velocity.y == 0.0f)
            jump = true;
            
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            rigidbody2D.velocity += Vector2.up * jumpVelocity;
            jump = false;
        }

        if (rigidbody2D.velocity.y < 0)
        {
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody2D.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
