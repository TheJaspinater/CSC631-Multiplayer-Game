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
        Debug.Log(transform.position);
        ServerSend.ProjectilePosition(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Initialize(Vector2 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer) 
    {
        initialForce = new Vector2(_initialMovementDirection.x * _initialForceStrength * Time.deltaTime, 0);
        thrownByPlayer = _thrownByPlayer;
    }

    private void Explode()
    {
        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _collider in _colliders)
        {
            if (_collider.CompareTag("Player"))
            {
                _collider.GetComponent<Player>().TakeDamage(explosionDamage);
            }
        }

        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(2f);

        Explode();
    }
}
