using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_CMovement : MonoBehaviour
{
    [Header("Movement:")]

    public float speed;
    [SerializeField] float crouchSpeed = 40;
    [SerializeField] float walkSpeed = 80;
        
    public float maxSpeed;
    [SerializeField] float maxCrouchSpeed = 7f;
    [SerializeField] float maxWalkSpeed = 14;
    [SerializeField] public float extraWindInertia = 0;
               
    [SerializeField] float groundDeceleration = 100;
    private float direction;    
    private float height;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);
    private bool crouching;


    [Header("Jump & Related:")]

    [SerializeField] private float jumpHeight = 20;
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

    [Header("Death")]
    [SerializeField] private bool isDead;
    [SerializeField] private bool isRespawning;
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] public Vector3 respawnPoint;
    [SerializeField] private float deadtime = 1;


    private void Awake()
    {   
        // Get needed components
        rB = GetComponent<Rigidbody2D>();
        animatorCharacter = GetComponent<Animator>();
        hook = GetComponent<S_CHook>();
    }

    public Vector2 GetInput()
    {
        //Convert axis raw of H and V to a GetInput Vector2
        return new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        if(movementEnabled)
        {
            // Process on a fixed Update all phisics related stuf
            MoveCharacter();
            ApplyDeacceleration();
            LastPositionChecker();

            directionMultyplayer = Mathf.Abs(direction) == 0 ? 0.5f : Mathf.Abs(direction);
            fallSpeedMultiplayer = parachuteDecendSpeed/directionMultyplayer;
            
            //Parachute falling speed diference
            //When you pres "E" the parachute activates but doesn't change you'r speed till you start losing height.
            if(parachute)
            {
                if(isLower == true)
                {
                    rB.velocity = new Vector2(rB.velocity.x, rB.velocity.y*-fallSpeedMultiplayer);
                }
                rB.gravityScale = fallGravityAir;
            }
        }
    }

    void AjustGameRotation() 
    {
        if (hook.isHooked)
        {
            rB.constraints = RigidbodyConstraints2D.None;
        }
        else 
        {
            rB.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Update()
    {
        //Check 4 Animations
        if (direction < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if(direction > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        Animations();
        if(movementEnabled)
        {
            hook.enabled = true;
            //JumpRelated
            Coyote();
            Buffer();            
            JumpChecks();
            AjustGameRotation();

            // When character is not in the air and buffer is runing chech if player has a possible jumc condition, if true jumps.
            if (bufferCounter > 0f && ((onGround || validCoyote || hook.isHooked) && !isJumping))
            {
                //We set the amount of time character will jump and if needed we dissabled the hook.
                limitJumpTime = limitJumpTimeValue;
                if (hook.isHooked) 
                {
                    hook.isHooked = false;
                    hook.grappleRope.enabled = false;
                    hook.m_springJoint2D.enabled = false;
                }

                //Jump
                JumpHability();
                
            }
            //If character stops pressing jump or any of the jump possibilitis are able character stops the jump.
            else if((!Input.GetButton("Jump") || (bufferCounter < 0f && onGround && !isJumping && hook.isHooked)))
            {
                //The player is no longer jumping, gravity is set to fall and jumptime resets.
                isJumping = false;
                rB.gravityScale = fallGravityLand;
                limitJumpTime = limitJumpTimeValue * 2;
            }

            // When "ctrl" is pressed and the player is on ground the player can crouch
            if(Input.GetButton("Crouch") && onGround)
            {
                crouching = true;
            }
            else
            {
                crouching = false;
            }

            //if the player isn't hooked or on ground when "E" is pressed opens parachute, if is already opened it gets closed. 
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
        else
        {
            hook.enabled = false;
        }
        

    }

    public void LastPositionChecker()
    {
        //We use this to check if player is falling by setting current position to current pos,
        //then checking if this positin is lower then last pos, after checkings we put the current
        //position in last position to have a last pos in next check

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
        //We detect player status and depending if input is sprint or crouch and we change speed and max speed acordingly

        if(crouching && !isJumping)
        {
            speed = Input.GetButton("Crouch") ? crouchSpeed : walkSpeed;            
            maxSpeed = Input.GetButton("Crouch") ? maxCrouchSpeed :maxWalkSpeed;   
        }
        else 
        {
            speed = walkSpeed;
            maxSpeed = maxWalkSpeed;
        }

        //Transform input in to a variable
        direction = GetInput().x;
        height = GetInput().y;

        //We add force in horizantal axis acording our speed
        rB.AddForce(new Vector2(direction, 0) * speed);

        //if the absulute speed on x is superior to max speed we include y velocity acording to x speed
        if(Mathf.Abs(rB.velocity.x) > maxSpeed + extraWindInertia)
        {
            rB.velocity = new Vector2(Mathf.Sign(rB.velocity.x) * maxSpeed, rB.velocity.y);
        }
    }

    public void ApplyDeacceleration()
    {
        //Applay drag
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
        //We are jumping and change the gravity to air
        isJumping = true;
        rB.gravityScale = fallGravityAir;
        AjustGameRotation();
        Jump();
    }

    public void JumpChecks()
    {
        //When its on ground parachute gets diactivated, gravity is land, resets coyote count and we are no longer jumping. 
        if (onGround)
        {
            parachute = false;
            rB.gravityScale = fallGravityLand;
            coyoteCount = 1;
            isJumping = false;
        }

        //Check if character is falling and aplay gravity depending of the coyote time
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
        //decrease jump time counter
        limitJumpTime -= 2 * Time.deltaTime; 

        //applay land fall gravity when character isn't jumping, coyote is not valid and isn't hooked
        if((limitJumpTime < 0f && !validCoyote) && !hook.isHooked)
        {
            rB.gravityScale = fallGravityLand;
        }
    }

    public void Jump()
    {
        //We clear y velocity befor aplayinf a impuls force upwards
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
    public void OnTriggerEnter2D(Collider2D Trig)
    {
        if(Trig.gameObject.tag == "Death")
        {
            DeadFunction();
        }
    }

    void DeadFunction()
    {
        isDead = true;
        movementEnabled = false;
        rB.drag = 100;
        rB.gravityScale = 0;
        Invoke("Respawn", deadtime);

    }

    void Respawn()
    {
        isRespawning = true;
        transform.position = respawnPoint;
        Invoke("RegainControll", deadtime);
        
    }

    void RegainControll()
    {
        isRespawning = false;
        isDead = false;
        movementEnabled = true;
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


        if (isDead)
        {
            if(!isRespawning)
            {
                animatorCharacter.Play("Death");
            }
            else
            {
                animatorCharacter.Play("Respawn");
            }      
        }
        else if (hook.isHooked)
        {
            animatorCharacter.Play("Grab");
        }
        else if (parachute) 
        {
            animatorCharacter.Play("Paravela");
        }
        else if (!isJumping && !onGround)
        {
            animatorCharacter.Play("ClimberFall");
        }
        else if (isJumping && !onGround)
        {
            animatorCharacter.Play("ClimberJump");
        }
        else if (direction != 0)
        {
            if (speed == crouchSpeed)
            {
                animatorCharacter.Play("ClimberCrouch");
            }
            else
            {
                animatorCharacter.Play("ClimberWalk");
            }
        }
        else
        {
            animatorCharacter.Play("Idle");
        } 
    }

}
