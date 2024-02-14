using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovement : MonoBehaviour
{
    float speed;
    float walkSpeed = 7;
    float sprintSpeed = 15;
    float walkAcceleration = 75;
    float airAcceleration = 50;
    float groundDeceleration = 200;
    float jumpHeight = 12;
    public float Cgravity;
    public float mayJump = 1;
    public bool alreadyjumped;

    private CapsuleCollider2D capCollider;

    private Vector2 velocity;

    public Vector2 collBox;
    public float castDistance;
    public LayerMask groundLayer; 

    public Animator animatorCharacter;

    private void Awake()
    {      
        capCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float moveInput = Input.GetAxisRaw("Horizontal");

        float acceleration = isGrounded() ? walkAcceleration : airAcceleration;
        float deceleration = isGrounded() ? groundDeceleration : 0;

        //Jump
        if (isGrounded())
        {
            velocity.y = 0;
            mayJump = 1;
        }
        else
        {
            mayJump -= Time.deltaTime;
            Cgravity = velocity.y;
            velocity.y += (Physics2D.gravity.y * 2) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded())
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                
            }
            else if(alreadyjumped == false && mayJump >=0)
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
            else
            {
                alreadyjumped = true;
            }
            
        }

        //Basic Movement
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
            animatorCharacter.SetBool("Static",false);
            animatorCharacter.SetBool("Walk",true);
            if(velocity.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else
        {
            animatorCharacter.SetBool("Walk",false);
            animatorCharacter.SetBool("Static",true);
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);

    }

    public bool isGrounded()
    {
        if(Physics2D.BoxCast(transform.position, collBox, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, collBox);
    }
}
