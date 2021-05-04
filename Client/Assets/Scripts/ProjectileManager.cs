using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int id;
    public GameObject explosionPrefab;

    private Vector3 lastPosition;
    private Vector3 velocity;

    public void Initialize(int _id)
    {
        id = _id;
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    public void Explode(Vector3 _position)
    {
        transform.position = _position;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }
}
