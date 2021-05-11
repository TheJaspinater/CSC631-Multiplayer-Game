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

    public int kills;

    private bool[] inputs;
    private bool[] attackInputs;

    private int height;
    private int width;
    private System.Random randomeNumber = new System.Random(DateTime.Now.ToString().GetHashCode());

    //TESTING---------------------
    private Vector2 _inputDirection;
    // private bool isWalking;
    private bool isFacingRight = true;

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    public float speed = 10f;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    public float jumpHeight = 5f;

    public float throwForce = 20f;

    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private Vector2 velocity;
    private Rigidbody2D body;

    private bool isGrounded;
    private int canJump = 0;

    private DateTime waitTime = System.DateTime.Now;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    public float attackRadius, attackDamage;
    public GameObject attackHitBoxPos;

    public GameObject firePoint;

    [SerializeField]
    private LayerMask whatIsDamageable;

    //----------------------------

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        height = GameObject.Find("WorldGen").GetComponent<MapGenV2>().height;
        width = GameObject.Find("WorldGen").GetComponent<MapGenV2>().width;
}

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        kills = 0;

        inputs = new bool[4];
        attackInputs = new bool[2];
    }

    public void playerDied()
    {
        health = 0f;
        //transform.position = new Vector3(150f, 150f, 0);
        //ServerSend.PlayerPosition(this);
        StartCoroutine(Respawn());
    }


    private void Update()
    {

    }


    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (transform.position.y < -10)
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
        if (attackInputs[1])
        {
            PlayerShoot(firePoint.transform.position);
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

        // firePoint.transform.Rotate(new Vector3(0, 180f, 0));
        // new Vector3(attackHitBoxPos.position.x * -1, attackHitBoxPos.position.y, attackHitBoxPos.position.z);
    }



    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    /// 
    private void Move(Vector2 _inputDirection)
    {
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
            body.velocity = new Vector2(_inputDirection.x * speed, body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(0 * speed, body.velocity.y);
        }

        isGrounded = false;

        ServerSend.PlayerPosition(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    public void PlayerAttack(bool[] _attackInputs)
    {
        attackInputs = _attackInputs;
    }

    public void PlayerShoot(Vector2 _shotDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (isFacingRight)
        {
            NetworkManager.instance.InstantiateProjectile(firePoint.transform).Initialize(_shotDirection, throwForce, id, this);
        }
        else
        {
            NetworkManager.instance.InstantiateProjectile(firePoint.transform).Initialize(-_shotDirection, throwForce, id, this);
        }

    }

    public int PlayerID()
    {
        return id;
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
    
    public void TakeDamage(float _attackDamage, Player _schoolShooter)
    {
        if (health <= 0f)
        {
            _schoolShooter.kills++;
            Debug.Log("player " + _schoolShooter.id + " has " + _schoolShooter.kills + " kills");
            // ServerSend.PlayerKills(_schoolShooter);
            Debug.Log("player " + id + " was killed");
            return;
        }

        health -= _attackDamage;
        if (health <= 0f)
        {
            health = 0f;
            // transform.position = new Vector3(150f, 150f, 0);
            // ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private void CheckAttackHitBox()
    {

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.transform.GetChild(0).gameObject.transform.position, attackRadius, whatIsDamageable);

        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.GetType() == typeof(BoxCollider2D))
            {
                Player _playerShot = collider.GetComponent<Player>();
                _playerShot.TakeDamage(attackDamage, this);
                Debug.Log("hit");

                if (_playerShot.health <= 0)
                {
                    kills++;
                    Debug.Log("player " + id + " has " + kills + " kills");
                    ServerSend.PlayerKills(this);

                }

            }

            else if (collider.GetComponent<Enemy>())
            {
                collider.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
            // collider.GetComponent<Player>().TakeDamage(attackDamage);

        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawWireSphere(attackHitBoxPos.transform.GetChild(0).gameObject.transform.position, attackRadius);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0);

        transform.position = findSpawnLocation();
        ServerSend.PlayerPosition(this);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }

    private Vector3 findSpawnLocation()
    {
        int wallCount = 0;

        for(int y = randomeNumber.Next(height - (height / 4), height); y > (height - (height / 4)); y--)
        {
            for (int x = randomeNumber.Next(0, width); x < width; x++)
            {
                wallCount = GameObject.Find("WorldGen").GetComponent<MapGenV2>().CountAdjacentTiles(x,y,1,1,1,2);
                Debug.Log("checking Respawn Location at " + x + " " + y + ". wallCount = " + wallCount + ".");
                if (wallCount - 1 == 0)//HArd coded. Uknown bug not returning 0
                {
                    return new Vector3(x, y, 0);
                }
            }
        }

        return new Vector3(width / 2, height + 10, 0);
    }
}