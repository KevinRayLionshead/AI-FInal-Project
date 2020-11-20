using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    [SerializeField]//the different colliders for crouching and standing
    BoxCollider2D normalBox;
    [SerializeField]
    public BoxCollider2D crouchBox;

    SpriteRenderer spriteRenderer;//to change the sprite for crouch and jumping

    [SerializeField]
    Sprite tallDino;//standing sprite
    [SerializeField]
    Sprite crouchDino;//crouch sprite

    bool crouchKey;
    public bool dead;

    public GameObject hazardManager;

    public bool GetCrouch()
    {
        return crouchKey;
    }
    public void SetCrouch(bool b)
    {
        crouchKey = b;
    }


    // Start is called before the first frame update
    void Start()
    {
        crouchKey = false;
        dead = false;

        rigidbody2D = GetComponent<Rigidbody2D>();//sets components to the attached ones
        spriteRenderer = GetComponent<SpriteRenderer>();

        normalBox.enabled = true;
        crouchBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.C) || crouchKey) && rigidbody2D.velocity.y == 0.0f)//where crouch is toggled, has to be on ground
        {
            spriteRenderer.sprite = crouchDino;//changes sprite to crouch 
            normalBox.enabled = false;//swaps colliderboxes
            crouchBox.enabled = true;
            
        }
        else if (!Input.GetKey(KeyCode.C) || !crouchKey || rigidbody2D.velocity.y != 0.0f)//untoggled crouch
        {
            spriteRenderer.sprite = tallDino;//changes sprite to standing
            normalBox.enabled = true;//swaps collider boxes back
            crouchBox.enabled = false;
            
        }

        if(dead)
        {
            for (int i = 0; i < hazardManager.GetComponent<Spawning>().gameObjects.Count; i++)
            {
                Destroy(hazardManager.GetComponent<Spawning>().gameObjects[i]);
                
            }
            hazardManager.GetComponent<Spawning>().gameObjects = new List<GameObject>();
            dead = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "enemy" || collision.tag == "Wide Enemy") && gameObject.tag == "Player")
        {
            Debug.Log("bonked");

            dead = true;
        }
    }
}
