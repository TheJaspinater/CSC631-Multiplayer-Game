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
            if (_collider.GetComponent<Player>().PlayerID() == thrownByPlayer)
                continue;
            else
            {
                Explode(_collider);
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

    public void Initialize(Vector2 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer) 
    {
        initialForce = new Vector2(_initialMovementDirection.x * _initialForceStrength * Time.deltaTime, 0);
        thrownByPlayer = _thrownByPlayer;
    }

    private void Explode(Collider2D collider)
    {
        ServerSend.ProjectileExploded(this);

        collider.GetComponent<Player>().TakeDamage(explosionDamage);
        Debug.Log("player hit");

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
