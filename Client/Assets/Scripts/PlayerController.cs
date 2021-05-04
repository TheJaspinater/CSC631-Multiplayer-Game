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
        SendInputToServer();
        SendAttackToServer();
        
    }

    private void FixedUpdate()
    {
        CheckMovementDirection();
        
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

    private void SendAttackToServer()
    {
        bool[] _attackInputs = new bool[]
        {
            Input.GetMouseButtonDown(0),
            Input.GetMouseButtonDown(1),
        };

        /*
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(true);
        }
        */
        
        ClientSend.PlayerAttack(_attackInputs);
    }

    public void CheckMovementDirection()
    {

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


}