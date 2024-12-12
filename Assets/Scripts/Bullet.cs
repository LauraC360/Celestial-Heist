using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject vfx;
    private SpaceshipController _spaceshipController;
    
    public void Fire(SpaceshipController spaceshipController, Vector3 position, float speed)
    {
        _spaceshipController = spaceshipController;
        
        transform.LookAt(position);
        rigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
        {
            Die();
            return;
        }
        
        var spaceship = other.attachedRigidbody.GetComponent<SpaceshipController>();
        if (spaceship != null && spaceship != _spaceshipController)
        {
            spaceship.Damage(25);
            Die();
        }
    }

    private void Die()
    {
        var deathVfx = Instantiate(vfx);
        deathVfx.transform.position = transform.position;
        Destroy(deathVfx, 5f);
        Destroy(gameObject);
    }
}
