using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovement : MonoBehaviour
{
    float speed;
    float walkSpeed = 40;
    float sprintSpeed = 80;

    float maxSpeed;
    float maxWalkSpeed = 7;
    float maxSprintSpeed = 14;
    float groundDeceleration = 100;

    public float direction;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);

    float jumpHeight = 1200;
    private float fallGravityAir = 5;
    private float fallGravityLand = 10;

    public float limitJumpTime = 10;
    public float mayJump = 1;
    public bool isJumping;
    public bool alreadyJumped;
    
    private bool onGround;

    public bool enableGroundCheck = true; 


    private Rigidbody2D rB;

    private Vector2 velocity;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer; 

    public Animator animatorCharacter;

    private void Awake()
    {      
        rB = GetComponent<Rigidbody2D>();
    }

    public Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        ApplyDeacceleration();
    }

    private void Update()
    {
        Animations();
        JumpHability();
    }

    public void Animations()
    {
        //Basic Animations
        if (direction != 0)
        {
            animatorCharacter.SetBool("Static",false);
            animatorCharacter.SetBool("Walk",true);
            if(direction < 0)
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
        }  
    }

    private void MoveCharacter(){
        if(onGround)
        {
            speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;            
            maxSpeed = Input.GetKey(KeyCode.LeftShift) ? maxSprintSpeed :maxWalkSpeed;            
        }

        direction = GetInput().x;

        rB.AddForce(new Vector2(direction, 0) * speed);

        if(Mathf.Abs(rB.velocity.x) > maxSpeed)
        {
            rB.velocity = new Vector2(Mathf.Sign(rB.velocity.x) * maxSpeed, rB.velocity.y);
        }
    }

    public void ApplyDeacceleration(){
        if(Mathf.Abs(direction) < 0.2f && onGround && !isJumping)
        {
            rB.drag = groundDeceleration;
        }
        else
        {
            rB.drag = 0;
        }
    }
    
    public void JumpHability()
    {
        if (Input.GetButtonDown("Jump")){
            Jump();
        }

        if(Input.GetButton("Jump"))
        {
            if(limitJumpTime > 0)
            {
                isJumping = true;
                limitJumpTime -= 10 * Time.deltaTime;            
            }
            else
            {
                Debugs();
                isJumping = false;
                rB.gravityScale = fallGravityAir;
                rB.drag = 0;
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            rB.gravityScale = fallGravityLand;
            limitJumpTime = 10;
        }

        if(onGround){
            rB.gravityScale = fallGravityAir;
        }
        else if(!isJumping)
        {
            rB.gravityScale = fallGravityLand;
        }
    }

    public void Jump(){
        if(onGround)
        {
            rB.velocity = new Vector2(rB.velocity.x, 0) * Time.deltaTime;
            rB.AddForce(transform.up * jumpHeight);
        }      
    }

    void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider != null)
        {
            foreach (ContactPoint2D hitpos in col.contacts)
            {
                if (hitpos.normal.y > 0)
                {
                    onGround = true;
                }
                if (hitpos.normal.y < 0)
                {
                    rB.gravityScale = fallGravityLand;
                }
            }
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        onGround = false;
    }

    void Debugs() 
    {
        if (isJumping) 
        {
            Debug.Log(transform.position.y);
        }
    }
}
