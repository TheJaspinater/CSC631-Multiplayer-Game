using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public ParticleSystem dust;
    public ParticleSystem slash;

    public int id;
    private Animator anim;
    public PlayerManager playerManager;

    private bool isWalking;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool hasJumped;
    private bool hasShot;
    private bool hasDied = false;

    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Start()
    {
        id = Client.instance.myId;
        anim = GetComponent<Animator>();
        lastPosition = GameManager.players[Client.instance.myId].transform.position;
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
        velocity = (GameManager.players[Client.instance.myId].transform.position - lastPosition) / Time.deltaTime;
        lastPosition = GameManager.players[Client.instance.myId].transform.position;

        CheckSurroundings();
    }

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

        CheckIfJumped();
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

    private void CheckIfJumped()
    {
        if (velocity.y > 0f)
        {
            hasJumped = true;
        }
        else if (velocity.y <= 0f)
        {
            hasJumped = false;
        }
    }

    private void CheckIfHasDied()
    {
        if (playerManager.health <= 0)
        {
            hasDied = true;
            anim.SetBool("isDead", hasDied);
        }
        else if(playerManager.health > 0)
        {
            hasDied = false;
            anim.SetBool("isDead", hasDied);
        }
    }


    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", velocity.x > 0.01f || velocity.x < -0.01f);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("hasJumped", hasJumped);
        anim.SetFloat("yVelocity", velocity.y);
        anim.SetBool("shoot1", hasShot);
        // anim.SetBool("isDead", hasDied);

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

    public bool getIfJumped()
    {
        return hasJumped;
    }
}