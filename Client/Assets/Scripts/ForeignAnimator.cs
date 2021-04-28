using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignAnimator : MonoBehaviour
{
    public ParticleSystem dust;

    private Animator anim;

    private bool isWalking;
    private bool isFacingRight = true;
    private bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        CheckMovementDirection();
        UpdateAnimations();
    }

    // instead of all being local, all animations will be done with GameManager.players[ID].position 
    // or something of the sort


    private void FixedUpdate()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // Debug.Log(velocity.x);

        CheckSurroundings();
    }

    //public void CheckVelocity

    public void CheckMovementDirection()
    {

        if (isFacingRight && velocity.x < 0)
        {
            Flip();
        }
        else if (!isFacingRight && velocity.x > 0)
        {
            Flip();
        }



        if (velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

    }


    private void Flip()
    {
        CreateDust();
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180.0f, 0.0f);
    }


    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        // anim.SetFloat("yVelocity", rb.velocity.y);
    }

    // Checks if Player is Grounded
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // Radius for Ground Check
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void CreateDust()
    {
        dust.Play();
        Debug.Log("dust happens");
    }
}