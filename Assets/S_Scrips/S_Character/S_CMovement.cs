using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_CMovement : MonoBehaviour
{

    //Movement
     
        //BaseSpeeds
    public float speed;
    float crouchSpeed = 20;
    float walkSpeed = 40;
    float sprintSpeed = 80;

        //Max Speeds
    public float maxSpeed;
    float maxCrouchSpeed = 3.5f;
    float maxWalkSpeed = 7;
    float maxSprintSpeed = 14;
        
        //Deceleration
    float groundDeceleration = 100;
    public float direction;    
    public float height;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);
    private bool crouching;

    //Jump

        //Base Height1200
    private float jumpHeight = 20;

        //Fall Speed
    private float fallGravityAir = 5;
    private float fallGravityLand = 15;

        //Holding Jump
    public float limitJumpTime = 1;

        //Coyote
    public bool isJumping;
    public float coyoteCount = 3f;
    public bool validCoyote;

    //Input Buffer
    private float bufferTime = 0.2f;
    public float bufferCounter;


    //Groud Checks
    public bool onGround;

    //Components
    private Rigidbody2D rB;
    private Animator animatorCharacter;
    public Text textDispaly;
    public S_CHook hook;

    //GameStuff
    public bool onIce;

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

        //JumpRelated
        Coyote();
        Buffer();            
        JumpChecks();

        if((bufferCounter > 0f && (onGround || validCoyote) && !isJumping))
        {
            limitJumpTime = 1;
            JumpHability();
        }
        else if((!Input.GetButton("Jump") || (bufferCounter < 0f && onGround && !isJumping)) && !hook.isHooked)
        {
            isJumping = false;
            rB.gravityScale = fallGravityLand;
            limitJumpTime = 2;
        }

        if(Input.GetButton("Crouch") && onGround)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
        
    }

    private void MoveCharacter(){
        
        if(!crouching)
        {
            speed = Input.GetButton("Run") ? sprintSpeed : walkSpeed;            
            maxSpeed = Input.GetButton("Run") ? maxSprintSpeed :maxWalkSpeed;
        }
        else if(crouching && !isJumping)
        {
            speed = Input.GetButton("Crouch") ? crouchSpeed : walkSpeed;            
            maxSpeed = Input.GetButton("Crouch") ? maxCrouchSpeed :maxWalkSpeed;   
        }

        direction = GetInput().x;
        height = GetInput().y;

        rB.AddForce(new Vector2(direction, 0) * speed);

        if(Mathf.Abs(rB.velocity.x) > maxSpeed)
        {
            rB.velocity = new Vector2(Mathf.Sign(rB.velocity.x) * maxSpeed, rB.velocity.y);
        }
    }

    public void ApplyDeacceleration()
    {
        if((Mathf.Abs(direction) < 0.2f && onGround && !isJumping && !onIce) || (Mathf.Abs(direction) < 0.2f && crouching && !isJumping))
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
        isJumping = true;
        rB.gravityScale = fallGravityAir;
        Jump();
    }

    void JumpChecks()
    {
        
        if (onGround)
        {
            rB.gravityScale = fallGravityLand;
            coyoteCount = 1;
            isJumping = false;
        }

        if ((!isJumping && !onGround) && !hook.isHooked)
        {
            if (validCoyote)
            {
                rB.gravityScale = fallGravityAir;
            }
            else
            {
                rB.gravityScale = fallGravityLand;
            }
        }

        limitJumpTime -= 2 * Time.deltaTime; 

        if((limitJumpTime < 0f && !validCoyote) && !hook.isHooked)
        {
            rB.gravityScale = fallGravityLand;
        }
    }

    public void Jump()
    {
        rB.velocity = new Vector2(rB.velocity.x, 0);
        rB.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);                           
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
            coyoteCount = 0;
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

                        if(col.collider.tag == "Ice")
                        {
                            onIce = true;
                        }
                        else
                        {
                            onIce = false; 
                        }
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

    public void Animations()
    {
        //Basic Animations
        if (direction != 0)
        {
            if(direction < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            if(speed == sprintSpeed)
            {
                animatorCharacter.SetBool("Runing",true);
                animatorCharacter.SetBool("Static",false);
                animatorCharacter.SetBool("Walk",false);
                animatorCharacter.SetBool("Crouching",false); 

            }
            else if(speed == crouchSpeed)
            {
                animatorCharacter.SetBool("Crouching",true);
                animatorCharacter.SetBool("Static",false);
                animatorCharacter.SetBool("Walk",false);
                animatorCharacter.SetBool("Runing",false);
            }
            else
            {
                animatorCharacter.SetBool("Walk",true);  
                animatorCharacter.SetBool("Static",false);
                animatorCharacter.SetBool("Crouching",false);
                animatorCharacter.SetBool("Runing",false); 
            }
        }
        else
        {
            animatorCharacter.SetBool("Runing",false); 
            animatorCharacter.SetBool("Crouching",false); 
            animatorCharacter.SetBool("Walk",false);
            animatorCharacter.SetBool("Static",true);
        } 

        if (onGround)
        {
            animatorCharacter.SetBool("Falling",false);
            animatorCharacter.SetBool("Jumped",false);
        }
        else if(!isJumping && !onGround)
        {
            animatorCharacter.SetBool("Falling",true);
            animatorCharacter.SetBool("Jumped",false);
        }
        else if(isJumping && !onGround)
        {
            animatorCharacter.SetBool("Jumped",true);
        }
    }
}
