using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public float health;
    public float maxHealth = 100f;

    private bool[] inputs;
    private bool[] attackInputs;

    //TESTING---------------------
    private Vector2 _inputDirection;
    // private bool isWalking;
    // private bool isFacingRight = true;

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    public float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    public float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    public float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    public float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    public float jumpHeight = 4;

    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private Vector2 velocity;

    private bool isGrounded;
    private int canJump = 0;

    private DateTime waitTime= System.DateTime.Now;
    // public float groundCheckRadius;
    // public LayerMask whatIsGround;
    // public Transform groundCheck;
    /*
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    */
    public float attackRadius, attackDamage;
    [SerializeField]
    private Transform attackHitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;

    //----------------------------

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[4];
        attackInputs = new bool[1];
    }

    public void playerDied()
    {
        health = 0f;
        transform.position = new Vector3(150f, 150f, 0);
        ServerSend.PlayerPosition(this);
        StartCoroutine(Respawn());
    }

    /*
    private void Update()
    {
        CheckMovementDirection();
    }
    */

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if(transform.position.y < -10)
        {
            playerDied();
        }

        if (health <= 0f)
        {
            return;
        }

        _inputDirection = Vector2.zero;
        if (inputs[0]) //w
        {
            _inputDirection.y += 1f;
        }
        if (inputs[1]) //s
        {
            _inputDirection.y -= 1f;
        }
        if (inputs[2]) //a
        {
            _inputDirection.x -= 1f;
        }
        if (inputs[3]) //d
        {
            _inputDirection.x += 1f;
        }

        if (attackInputs[0]) // Left Mouse Click
        {
            CheckAttackHitBox();
        }

        //CheckSurroundings();
        Move(_inputDirection);
    }

    /*
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

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        attackHitBoxPos.Rotate(0f, 180.0f, 0.0f);
    }
    */


    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    /// 
    /*
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.up * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (isGrounded)
        {
            yVelocity = 0f;
            if (inputs[0])
            {
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity += gravity;
        }

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
    }
    */
    private void Move(Vector2 _inputDirection)
    {


        if (isGrounded)
        {
            velocity.y = 0;
            canJump = 0;
        }

        if (_inputDirection.y > 0 && canJump < 2)
        {
            // Calculate the velocity required to achieve the target jump height.
            TimeSpan result = System.DateTime.Now - waitTime;
            int milliseconds = (int)result.TotalMilliseconds;
            if (milliseconds > 400)
            {
                waitTime = System.DateTime.Now;
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                canJump += 1;
            }
            
        }

        float acceleration = isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : 0;

        if (_inputDirection.x != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * _inputDirection.x, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (velocity.y > -30)
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;

        }

        transform.Translate(velocity * Time.deltaTime);

        isGrounded = false;

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    isGrounded = true;
                    canJump = 0;
                }
            }
        }
        ServerSend.PlayerPosition(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    public void PlayerAttack (bool[] _attackInputs)
    {
        attackInputs = _attackInputs;
    }
    public void TakeDamage(float _attackDamage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _attackDamage;
        if (health <= 0f)
        {
            health = 0f;
            transform.position = new Vector3(150f, 150f, 0);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private void CheckAttackHitBox()
    {
        
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attackRadius, whatIsDamageable);

        foreach (Collider2D collider in detectedObjects)
        {
            // collider.transform.parent.SendMessage("Damage", attackDamage);
            collider.GetComponent<Player>().TakeDamage(attackDamage);
            Debug.Log("hit");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }
}