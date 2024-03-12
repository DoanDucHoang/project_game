using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D coll;
    private float dirx = 0f;
    private bool moveLeft;
    private bool moveRight;
    private int direction = 1;
    private enum MovementState {idle, running, jumping, falling, forcing}
    Vector3 lastVelocity;
    [SerializeField] private PhysicsMaterial2D bounceMat, normalMat;
    [SerializeField] private bool canJump;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 0.0f;
    [SerializeField] private AudioSource jumpSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        moveLeft = false;
        moveRight = false;
        canJump = false;
    }

    public void PointerDownLeft()
    {
        moveLeft = true;
    }

    public void PointerUpLeft()
    {
        moveLeft = false;
    }

    public void PointerDownRight()
    {
        moveRight = true;
    }

    public void PointerUpRight()
    {
        moveRight = false;
    }

    public void PointerDownJump()
    {
        canJump = true;
    }

    public void PointerUpJump()
    {
        canJump = false;
    }

    private void FixedUpdate()
    {
        // dirx = Input.GetAxisRaw("Horizontal");
        lastVelocity = rb.velocity;
        PlayerMove();
        Flip();
        UpdateAnimatorPlayer();
        PlayerJump();
    }

    private void PlayerMove()
    {
        // dirx = Input.GetAxisRaw("Horizontal");
        if(jumpForce == 0.0f && IsGrounded())
        {
            if (moveLeft)
            {
                dirx = -1;
            }
            else if (moveRight)
            {
                dirx = 1;
            }
            else
            {
                dirx = 0;
            }
            rb.velocity = new Vector2(dirx * moveSpeed, rb.velocity.y);
        }
    }

    private void Flip()
    {
        if(dirx * direction < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            direction *= -1;
        }
    }

    private void PlayerJump()
    {
        // if(Input.GetKey("space") && IsGrounded())
        if(canJump && IsGrounded())
        {
            jumpForce += 0.5f;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            rb.sharedMaterial = bounceMat;
            canJump = true;
        }
        else
        {
            canJump = false;
        }
        // if(Input.GetKey("space") && IsGrounded() && jumpForce >= 25.0f || Input.GetKey("space") == false && jumpForce >= 0.1f)
        if(canJump && IsGrounded() && jumpForce >= 25.0f || canJump == false && jumpForce >= 0.1f)
        {
            float tempX = dirx * moveSpeed;
            float tempY = jumpForce;
            rb.velocity = new Vector2(tempX,tempY);
            Invoke("ResetJump", 0.025f);
        }
        if (rb.velocity.y <= -1)
        {
            rb.sharedMaterial = normalMat;
        }
    }

    private void ResetJump()
    {
        jumpForce = 0.0f;
    }

    private void UpdateAnimatorPlayer()
    {
        MovementState state;
        if (dirx > 0f && canJump == false) {
            state = MovementState.running;
        }
        else if (dirx < 0f && canJump == false) {
            state = MovementState.running;
        }
        else {
            state = MovementState.idle;
        }

        if (canJump && IsGrounded()) 
        {
            state = MovementState.forcing;
        }
        else if (rb.velocity.y > .1f) 
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f && !IsGrounded())
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(rb.velocity.y <= -1)
        {
            var speed = lastVelocity.magnitude;
            var directionBounce = Vector3.Reflect(lastVelocity.normalized, coll.contacts[0].normal);
            rb.velocity = directionBounce * Mathf.Max(speed, 0f);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
