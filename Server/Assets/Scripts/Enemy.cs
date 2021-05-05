using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static int maxEnemies = 10;
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    private static int nextEnemyId = 1;

    public int id;
    public EnemyState state;
    public Player target;
    // CharacterController
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 8f;
    public float health;
    public float maxHealth = 100f;
    public float detectionRange = 30f;
    public float attackRange = 2f;
    public float attackAccuracy = 0.1f;
    public float patrolDuration = 3f;
    public float idleDuration = 1f;

    private bool isPatrolRoutineRunning;
    private float yVelocity = 0;


    private void Start()
    {
        id = nextEnemyId;
        nextEnemyId++;
        enemies.Add(id, this);

        ServerSend.SpawnEnemy(this);

        state = EnemyState.patrol;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        patrolSpeed *= Time.fixedDeltaTime;
        chaseSpeed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyState.idle:
                LookForPlayer();
                break;
            case EnemyState.patrol:
                if (!LookForPlayer())
                {
                    Patrol();
                }
                break;
            case EnemyState.chase:
                Chase();
                break;
            case EnemyState.attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private bool LookForPlayer()
    {
        foreach(Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Vector3 _enemyToPlayer = _client.player.transform.position - transform.position;
                if (_enemyToPlayer.magnitude <= detectionRange)
                {
                    // check whether checker has collided
                    // check whether collider hit is player
                    // change state from patrol running to chase
                    // stop coroutine when target found
                    // return true
                }
            }
        }
        return false;
    }

    private void Patrol()
    {
        if (!isPatrolRoutineRunning)
        {
            StartCoroutine(StartPatrol());
        }

        Move(transform.up, patrolSpeed);
    }

    private IEnumerator StartPatrol()
    {
        isPatrolRoutineRunning = true;
        Vector3 _randomPatrolDirection = Random.insideUnitCircle.normalized;
        transform.up = new Vector3(_randomPatrolDirection.x, _randomPatrolDirection.y, 0f);

        yield return new WaitForSeconds(patrolDuration);

        state = EnemyState.idle;

        yield return new WaitForSeconds(idleDuration);

        state = EnemyState.patrol;
        isPatrolRoutineRunning = false;
    }

    private void Chase()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;

            if (_enemyToPlayer.magnitude <= attackRange)
            {
                state = EnemyState.attack;
            }
            else
            {
                Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }
    
    private void Attack()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;
            transform.up = new Vector3(_enemyToPlayer.x, _enemyToPlayer.y, 0f);

            if (_enemyToPlayer.magnitude <= attackRange)
            {
                Attack(_enemyToPlayer);
            }
            else
            {
                Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }

    private void Move(Vector3 _direction, float _speed)
    {
        // fill in with move logic from Player.cs

        ServerSend.EnemyPosition(this);
    }

    private void Attack(Vector3 _attackDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _attackDirection, out RaycastHit _hit, attackRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                if (Random.value <= attackAccuracy)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50f);
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;

            enemies.Remove(id);
            Destroy(gameObject);
        }

        ServerSend.EnemyHealth(this);
    }

    private bool CanSeeTarget()
    {
        if (target == null)
        {
            return false;
        }

        // check if detectionRange still collides with anything
        // check if collided object is player
        // return true

        return false;
    }
}

public enum EnemyState
{
    idle,
    patrol,
    chase,
    attack
}