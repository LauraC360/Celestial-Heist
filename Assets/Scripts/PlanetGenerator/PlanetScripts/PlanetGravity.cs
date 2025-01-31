using System;
using System.Collections;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    [SerializeField] private SphereCollider gravityCollider;
    [SerializeField] private float minGravity;
    [SerializeField] private float maxGravity;
    
    private float gravityStrength = 9.81f;

    private Rigidbody _spaceshipRb;
    private PlayerController _playerController;
    private WorldMover _worldMover;
    
    private Coroutine _coroutine;
    
    public void Setup(float planetRadius, float gravityStrength)
    {
        this.gravityStrength = gravityStrength * maxGravity + (1 - gravityStrength) * minGravity;
        gravityCollider.radius = planetRadius * 1.6f;
    }

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (!rb.CompareTag("Player"))
            return;
        
        _spaceshipRb = rb;
        _worldMover = rb.GetComponent<WorldMover>();
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (!rb.CompareTag("Player"))
            return;
        
        _spaceshipRb.transform.SetParent(null);
        _worldMover.enabled = true;
        
        _spaceshipRb = null;
        _playerController = null;
        _worldMover = null;
    }

    private void OnCollisionEnter(Collision other)
    {
        var rb = other.rigidbody;
        if (!rb.CompareTag("Player"))
            return;
        
        _playerController = rb.gameObject.GetComponentInChildren<PlayerController>();
        _worldMover.enabled = false;
        
        _spaceshipRb.transform.SetParent(transform);
        
        if(_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private void FixedUpdate()
    {
        ApplyGravityToShip();
        ApplyGravityToPlayer();
    }

    private void ApplyGravityToShip()
    {
        if (_spaceshipRb == null)
            return;
        
        var dir = _spaceshipRb.transform.position - transform.position;
        dir.Normalize();
        
        _spaceshipRb.AddForce(-dir * maxGravity, ForceMode.Acceleration);
    }

    private void ApplyGravityToPlayer()
    {
        if (_playerController == null)
            return;
        
        
    }
}