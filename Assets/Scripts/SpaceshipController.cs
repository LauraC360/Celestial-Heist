using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float brakeMultiplier;
    [SerializeField] private float pitchMultiplier;
    [SerializeField] private float yawMultiplier;
    [SerializeField] private float rollMultiplier;
    [SerializeField] private Bullet laserPrefab;
    [SerializeField] private Transform[] pistols;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private Radar frontRadar;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private float health = 1000f;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private AudioSource engineSound;

    private Rigidbody _rigidbody;
    private bool _dead = false;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(FiringCoroutine());
    }

    private void Update()
    {
        if (engineSound != null)
        {
            engineSound.volume = inputManager.Thrust() * 0.075f + 0.05f;
            engineSound.pitch = inputManager.Thrust() * 0.7f + 0.9f;
        }
        
        _rigidbody.AddForce(transform.rotation * Vector3.forward * (inputManager.Thrust() * thrustMultiplier), ForceMode.Acceleration);
        if(_rigidbody.velocity.magnitude > 0)
            _rigidbody.AddForce(-_rigidbody.velocity.normalized * (inputManager.Airbrake() * brakeMultiplier), ForceMode.Acceleration);
        
        _rigidbody.AddTorque(transform.rotation * Vector3.right * _rigidbody.velocity.magnitude * (inputManager.Pitch() * pitchMultiplier), ForceMode.Acceleration);
        _rigidbody.AddTorque(transform.rotation * Vector3.up * _rigidbody.velocity.magnitude * (inputManager.Yaw() * yawMultiplier), ForceMode.Acceleration);
        _rigidbody.AddTorque(transform.rotation * Vector3.forward * _rigidbody.velocity.magnitude * (inputManager.Roll() * rollMultiplier), ForceMode.Acceleration);
        
        if(frontRadar == null)
            return;
        
        var targetShip = frontRadar.spaceships.Count > 0 ? frontRadar.spaceships[0] : null;
        crosshair.SetToTarget(targetShip ? targetShip.transform : null);
    }

    private IEnumerator FiringCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => inputManager.Fire());
            
            var targetShip = frontRadar.spaceships.Count > 0 ? frontRadar.spaceships[0] : null;
        
            for (int i = 0; i < pistols.Length; i++)
            {
                var target = transform.position + transform.forward * 1000;
                if (targetShip != null)
                {
                    Vector3 enemyPosition = targetShip.transform.position;
                    Rigidbody enemyRigidbody = targetShip.GetComponent<Rigidbody>();
                    Vector3 enemyVelocity = enemyRigidbody.velocity;

                    Vector3 directionToTarget = enemyPosition - pistols[i].position;
        
                    float timeToReachTarget = directionToTarget.magnitude / bulletSpeed;

                    target = enemyPosition + enemyVelocity * timeToReachTarget;
                }
                
                pistols[i].LookAt(target);
                
                var laser = Instantiate(laserPrefab);
                laser.transform.position = pistols[i].position;
                laser.Fire(this, target, bulletSpeed);
            }
            
            yield return new WaitForSeconds(1 / fireRate);
        }
    }

    public void Damage(float value)
    {
        if(_dead)
            return;
        
        health -= value;
        
        if(health <= 0)
            Die();
    }

    private void Die()
    {
        _dead = true;
        var deathVfx = Instantiate(this.deathVfx);
        deathVfx.transform.position = transform.position;
        Destroy(deathVfx, 5f);
        Destroy(gameObject);
    }
}
