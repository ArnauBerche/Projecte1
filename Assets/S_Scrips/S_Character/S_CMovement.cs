using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_CMovement : MonoBehaviour
{
    [Header("Movement:")]
        
    public float speed;
    [SerializeField] float crouchSpeed = 20;
    [SerializeField] float walkSpeed = 40;
    [SerializeField] float sprintSpeed = 80;
        
    public float maxSpeed;
    [SerializeField] float maxCrouchSpeed = 3.5f;
    [SerializeField] float maxWalkSpeed = 7;
    [SerializeField] float maxSprintSpeed = 14;
               
    [SerializeField] float groundDeceleration = 100;
    private float direction;    
    private float height;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);
    private bool crouching;


    [Header("Jump & Related:")]

    [SerializeField] private float jumpHeight = 30;
    [SerializeField] private float limitJumpTime = 1;
    [SerializeField] private float limitJumpTimeValue = 1;

    [SerializeField] private float fallGravityAir = 5;
    [SerializeField] private float fallGravityLand = 15;

    [SerializeField] private float coyoteCount = 3f;
    [SerializeField] private float bufferTime = 0.2f;
    private float bufferCounter;


    [Header("Parachute:")]

    [SerializeField] private float parachuteDecendSpeed;
    private float directionMultyplayer;
    private float fallSpeedMultiplayer;


    [Header("Components:")]

    [SerializeField] private Rigidbody2D rB;
    [SerializeField] private Animator animatorCharacter;
    [SerializeField] private Text textDispaly;
    [SerializeField] private S_CHook hook;
    

    [Header("Game Checks")]
        
    [SerializeField] private bool onGround;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool validCoyote;
    [SerializeField] private bool onIce;
    [SerializeField] public bool parachute = false;
    [SerializeField] private Vector2 lastPos;
	[SerializeField] private Vector2 currentPos;
    [SerializeField] private bool isLower;


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
        LastPositionChecker();
        directionMultyplayer = Mathf.Abs(direction) == 0 ? 0.5f : Mathf.Abs(direction*2);
        fallSpeedMultiplayer = parachuteDecendSpeed/directionMultyplayer;
        if(parachute)
        {
            if(isLower == true)
            {
                rB.velocity = new Vector2(rB.velocity.x, -fallSpeedMultiplayer);
            }
            rB.gravityScale = fallGravityAir;
        }
    }

    private void Update()
    {
        Animations();

        //JumpRelated
        Coyote();
        Buffer();            
        JumpChecks();

        if(bufferCounter > 0f && ((onGround || validCoyote || hook.isHooked) && !isJumping))
        {
            limitJumpTime = limitJumpTimeValue;
            hook.isHooked = false;
            hook.grappleRope.enabled = false;
            hook.m_springJoint2D.enabled = false;
            JumpHability();
            
        }
        else if((!Input.GetButton("Jump") || (bufferCounter < 0f && onGround && !isJumping && hook.isHooked)))
        {
            isJumping = false;
            rB.gravityScale = fallGravityLand;
            limitJumpTime = limitJumpTimeValue * 2;
        }

        if(Input.GetButton("Crouch") && onGround)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        if(Input.GetButtonDown("Parachute") && !onGround && !hook.isHooked)
        {
            if(parachute)
            {
                parachute = false;
            }
            else
            {
                parachute = true;
            }
        }

    }

    public void LastPositionChecker()
    {
        currentPos = transform.position;

		if (currentPos.y < lastPos.y) 
        {
			isLower = true;
		} 
        else 
        {
			isLower = false;
		}

		lastPos = currentPos;
    }

    public void MoveCharacter()
    {
        
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

    public void JumpChecks()
    {
        
        if (onGround)
        {
            parachute = false;
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
    public void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider != null)
        {
            foreach (ContactPoint2D hitpos in col.contacts)
            {
                parachute = false;
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
    public void OnCollisionExit2D(Collision2D col)
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
