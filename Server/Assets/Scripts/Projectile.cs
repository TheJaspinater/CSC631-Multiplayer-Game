using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody2D rb;
    public int thrownByPlayer;
    public Player originalPlayer;

    public Vector2 initialForce;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 50f;

    [SerializeField]
    private LayerMask whatIsDamageable;

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this, thrownByPlayer);

        rb.velocity = initialForce;
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        // Debug.Log(transform.position);
        ServerSend.ProjectilePosition(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Collider2D[] _colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatIsDamageable);
        foreach (Collider2D _collider in _colliders)
        {
            if (_collider.GetComponent<BoxCollider2D>())
            {
                if (_collider.GetComponent<Player>().PlayerID() == thrownByPlayer)
                    continue;
                else
                {
                    Explode(_collider);            
                    // ServerSend.PlayerKills(originalPlayer);
                }
            }


            /*
            if (_collider.CompareTag("Player"))
            {
                Debug.Log("player shot");
                _collider.GetComponent<Player>().TakeDamage(explosionDamage);
            }
            */
        }
    }

    public void Initialize(Vector2 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer, Player _thrownBy)
    {
        initialForce = new Vector2(_initialMovementDirection.x * _initialForceStrength * Time.deltaTime, 0);
        thrownByPlayer = _thrownByPlayer;
        originalPlayer = _thrownBy;
    }

    private void Explode(Collider2D collider)
    {
        ServerSend.ProjectileExploded(this);

        Player _playerShot = collider.GetComponent<Player>();
        _playerShot.TakeDamage(explosionDamage, originalPlayer);
        Debug.Log("player shot");
        if (_playerShot.health <= 0)
        {
            ServerSend.PlayerKills(originalPlayer);
        }

        /*
        if (_playerShot.health <= 0)
        {
            _playerKills++;
            Debug.Log("player " + id + " has " + kills + " kills");

        }
        */
        // collider.GetComponent<Player>().TakeDamage(explosionDamage);
        // Debug.Log("player hit");

        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private void Explode()
    {
        ServerSend.ProjectileExploded(this);

        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(2f);

        Explode();
    }
}
