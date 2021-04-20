using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool isWalking;
    // private bool isFacingRight = true;
    private bool isGrounded;

    private void Update()
    {
        CheckMovementDirection();
    }

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

        bool[] _attackInputs = new bool[]
        {
            Input.GetMouseButtonDown(0),
        };
        
        ClientSend.PlayerMovement(_inputs);
        ClientSend.PlayerAttack(_attackInputs);
    }

    public void CheckMovementDirection()
    {
        /*
        if (isFacingRight && Input.GetKey(KeyCode.D))
        {
            Debug.Log("going right");
            //Flip();
        }
        else if (!isFacingRight && Input.GetKey(KeyCode.A))
        {
            Debug.Log("going left");
            //Flip();
        }
        */
        
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

    /*
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180.0f, 0.0f);
    }
    */


}