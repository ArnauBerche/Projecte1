using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovement : MonoBehaviour
{

    //Movement
     
        //BaseSpeeds
    float speed;
    float walkSpeed = 40;
    float sprintSpeed = 80;

        //Max Speeds
    float maxSpeed;
    float maxWalkSpeed = 7;
    float maxSprintSpeed = 14;
        
        //Deceleration
    float groundDeceleration = 100;


    public float direction;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);

    //Jump

        //Base Height
    private float jumpHeight = 1200;

        //Fall Speed
    private float fallGravityAir = 5;
    private float fallGravityLand = 10;

        //Holding Jump
    private float limitJumpTime = 10;

        //Coyote
    private bool isJumping;
    private float coyoteCount = 0.5f;
    private bool validCoyote;

    //Input Buffer
    private float bufferTime = 1;
    public float bufferCounter;


    //Groud Checks
    private bool onGround;

    //Components
    private Rigidbody2D rB;
    private Animator animatorCharacter;

    private void Awake()
    {      
        rB = GetComponent<Rigidbody2D>();
        animatorCharacter = GetComponent<Animator>();
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
        Buffer();
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
        if ((bufferCounter > 0f && (validCoyote || onGround))) 
        {
            Jump();
            bufferCounter = 0;
        }

        if (Input.GetButton("Jump"))
        {
            if (limitJumpTime > 0)
            {
                isJumping = true;
                limitJumpTime -= 10 * Time.deltaTime;
            }
            else
            {
                coyoteCount = 0;
                isJumping = false;
                rB.gravityScale = fallGravityAir;
                rB.drag = 0;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            coyoteCount = 0;
            isJumping = false;
            rB.gravityScale = fallGravityLand;
            limitJumpTime = 10;
        }

        if(onGround){
            rB.gravityScale = fallGravityAir;
            coyoteCount = 1;
        }
        else if(!isJumping)
        {
            if (validCoyote)
            {
                rB.gravityScale = fallGravityAir;
            }
            else
            {
                rB.gravityScale = fallGravityLand;
            }
            
            Coyote();
        }
    }
    public void Jump()
    {
            bufferCounter = 0f;
            rB.velocity = new Vector2(rB.velocity.x, 0) * Time.deltaTime;
            rB.AddForce(transform.up * jumpHeight);   
    }
    public void Coyote()
    {
        coyoteCount -= 10 * Time.deltaTime;
        if (coyoteCount > 0)
        {
            validCoyote = true;
        }
        else 
        {
            validCoyote = false;
        }
    }
    public void Buffer() 
    {
        if (Input.GetButtonDown("Jump"))
        {
            bufferCounter = bufferTime;
        }
        else
        {
            bufferCounter -= Time.deltaTime;
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
}
