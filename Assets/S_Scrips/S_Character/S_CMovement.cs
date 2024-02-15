using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovement : MonoBehaviour
{
    float speed;
    float walkSpeed = 40;
    float sprintSpeed = 80;

    float maxSpeed;
    float maxWalkSpeed = 5;
    float maxSprintSpeed = 10;
    float groundDeceleration = 100;

    public float direction;    
    private bool changedirection => (rB.velocity.x > 0f && direction < 0) ||  (rB.velocity.x < 0f && direction > 0);

    float jumpHeight = 20;
    public float fallGravityM = 1;
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
        Debug.Log(onGround);
    }

    private void Update()
    {
        Animations();
        JumpHability();
        onGround = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
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
            fallGravityM = 1;
        }

        if(Input.GetButton("Jump"))
        {
            isJumping = true;
            limitJumpTime -= 0.1f;
            
            if(limitJumpTime < 0)
            {
                isJumping = false;
                rB.gravityScale = fallGravityM;
                rB.drag = 0;
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            rB.gravityScale = fallGravityM;
        }

        if(onGround){
            fallGravityM = 1;
            rB.gravityScale = fallGravityM;
            limitJumpTime = 10;
        }
        else if(!isJumping)
        {
            fallGravityM = 8;
            rB.gravityScale = fallGravityM;
        }
    }

    public void Jump(){
        if(onGround)
        {
            rB.velocity = new Vector2(rB.velocity.x, 0);
            rB.velocity = new Vector2(rB.velocity.x, jumpHeight);
        }      
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
    }
}
