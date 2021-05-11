using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignAnimator : MonoBehaviour
{
    public ParticleSystem dust;
    public ParticleSystem slash;

    public int id;
    private Animator anim;
    public PlayerManager playerManager;

    private bool isWalking;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool hasShot;
    private bool hasDied = false;

    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Start()
    {
        id = playerManager.id;
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        CheckMovementDirection();
        CheckIfHasDied();
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

    public void isShooting(int _id, bool _shoot)
    {
        if (_id == id)
        {
            hasShot = _shoot;
            Debug.Log(hasShot);
        }
        Debug.Log("reached this");
    }

    public void finishShooting()
    {
        hasShot = false;
        anim.SetBool("shoot1", false);
    }

    private void Flip()
    {
        CreateDust();
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180.0f, 0.0f);
    }

    private void CheckIfHasDied()
    {
        if (playerManager.health <= 0)
        {
            hasDied = true;
            anim.SetBool("isDead", hasDied);
        }
        else if (playerManager.health > 0)
        {
            hasDied = false;
            anim.SetBool("isDead", hasDied);
        }
    }


    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        // anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("shoot1", hasShot);
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

    public void CreateSlash()
    {
        slash.Play();
    }
}