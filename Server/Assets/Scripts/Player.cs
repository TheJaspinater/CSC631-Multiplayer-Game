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
    public float lives;
    public float maxLives = 3;

    private bool[] inputs;
    private bool[] attackInputs;

    //TESTING---------------------
    private Vector2 _inputDirection;
    // private bool isWalking;
    private bool isFacingRight = true;

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    public float speed = 10f;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    public float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    public float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    public float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    public float jumpHeight = 5f;

    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private Vector2 velocity;
    private Rigidbody2D body;

    private bool isGrounded;
    private int canJump = 0;

    private DateTime waitTime= System.DateTime.Now;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        Debug.Log(isGrounded);
    }
    
    public float attackRadius, attackDamage;
    public GameObject attackHitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;

    //----------------------------

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        lives = maxLives;

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

    
    private void Update()
    {
        
    }
    

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

        CheckSurroundings();
        Move(_inputDirection);
        CheckMovementDirection();
        // CheckCollisions();
    }

    
    public void CheckMovementDirection()
    {

        if (isFacingRight && _inputDirection.x < 0)
        {
            Flip();
        }
        else if (!isFacingRight && _inputDirection.x > 0)
        {
            Flip();
        }

    }
    

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Debug.Log("has flipped");
        attackHitBoxPos.transform.Rotate(new Vector3(0, 180f, 0));
        // new Vector3(attackHitBoxPos.position.x * -1, attackHitBoxPos.position.y, attackHitBoxPos.position.z);
    }
    


    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    /// 
    private void Move(Vector2 _inputDirection)
    {

        /*
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
        

        body.MovePosition(new Vector2((transform.position.x + _inputDirection.x * speed * Time.deltaTime),
            transform.position.y + _inputDirection.y * speed * Time.deltaTime));
        */
        if (isGrounded)
        {
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
                body.velocity = new Vector2(body.velocity.x, jumpHeight);
                canJump += 1;
            }

        }
        
        if (_inputDirection.x != 0)
        {
            body.velocity = new Vector2(_inputDirection.x * speed, body.velocity.y) ;
        }
        else
        {
            body.velocity = new Vector2(0 * speed, body.velocity.y);
        }

        isGrounded = false;

        ServerSend.PlayerPosition(this);
    }

    /*
    // Retrieve all colliders we have intersected after velocity has been applied.
    public void CheckCollisions()
    {
        Collider2D[] circleHits = Physics2D.OverlapCircleAll((Vector2)transform.position + circleCollider.offset, circleCollider.radius);
        Collider2D[] boxHits = Physics2D.OverlapBoxAll((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0);

        foreach (Collider2D hit in circleHits)
        {
            // Ignore our own collider.

            if (hit == circleCollider)
                continue;
            if (hit == boxCollider)
                continue;


            ColliderDistance2D colliderDistance = hit.Distance(circleCollider);

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

        foreach (Collider2D hit in boxHits)
        {
            // Ignore our own collider.

            if (hit == circleCollider)
                continue;
            if (hit == boxCollider)
                continue;


            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object above us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y > 0)
                {
                    isGrounded = true;
                    // canJump = 0;
                }
            }
        }
    }
    */

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
        
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.transform.GetChild(0).gameObject.transform.position, attackRadius, whatIsDamageable);

        foreach (Collider2D collider in detectedObjects)
        {
            // collider.transform.parent.SendMessage("Damage", attackDamage);
            collider.GetComponent<Player>().TakeDamage(attackDamage);
            Debug.Log("hit");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawWireSphere(attackHitBoxPos.transform.GetChild(0).gameObject.transform.position, attackRadius);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }
}