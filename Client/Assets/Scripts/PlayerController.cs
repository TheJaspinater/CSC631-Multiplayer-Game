using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool isWalking;
    // private bool isFacingRight = true;
    private bool isGrounded;
    public string jumpSoundEvent;

    private void Update()
    {
        //CheckMovementDirection();
    }

    private void FixedUpdate()
    {
        CheckMovementDirection();
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

        if (Input.GetKeyDown(KeyCode.W)){
            FMODUnity.RuntimeManager.PlayOneShot(jumpSoundEvent);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isWalking = true;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            isWalking = false;
        }
        else
        {
            isWalking = false;
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