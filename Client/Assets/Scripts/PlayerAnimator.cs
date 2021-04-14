using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    private bool isWalking;
    private bool isFacingRight = true;
    private bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckMovementDirection();
        UpdateAnimations();
    }

    // instead of all being local, all animations will be done with GameManager.players[ID].position 
    // or something of the sort

    /*
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
        };

        ClientSend.PlayerMovement(_inputs);
    }
    */

    public void CheckMovementDirection()
    {
        
        if (isFacingRight && Input.GetKey(KeyCode.A))
        {
            Debug.Log("going right");
            Flip();
        }
        else if (!isFacingRight && Input.GetKey(KeyCode.D))
        {
            Debug.Log("going left");
            Flip();
        }
        

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isWalking = true;
            Debug.Log(isWalking);
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            isWalking = false;
            Debug.Log(isWalking);
        }
        else
        {
            isWalking = false;
            Debug.Log(isWalking);
        }

    }

    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180.0f, 0.0f);
    }
    

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        // anim.SetFloat("yVelocity", rb.velocity.y);
    }


}