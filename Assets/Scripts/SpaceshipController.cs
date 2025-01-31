using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using DG.Tweening;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float overdriveSpeed;
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
    [SerializeField] private AudioSource boosterSound;
    [SerializeField] private AudioSource boosterActivation;
    [SerializeField] private AudioSource boosterDeactivation;
    [SerializeField] private ParticleSystem boosterVfx;
    [SerializeField] private int team;

    public int Team => team;

    public float OverdriveValue => _overdriveTweenValue;

    [HideInInspector] public int lapNumber;
    [HideInInspector] public int chekpointIndex;

    private Rigidbody _rigidbody;
    private bool _dead = false;

    private bool _overdrive = false;
    private Tween _soundTween;

    private const float BoosterMaxVolume = 0.5f;

    private float _overdriveTweenValue = 0f;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(FiringCoroutine());

        lapNumber = 1;

        chekpointIndex = 0;
    }

    private void Update()
    {
        var thrust = inputManager.Thrust();
        CheckEngineSound();

        var forwardSpeed = inputManager.Overdrive() ? overdriveSpeed : thrust * thrustMultiplier;
        _rigidbody.AddForce(transform.rotation * Vector3.forward * forwardSpeed, ForceMode.Acceleration);
        if(_rigidbody.velocity.magnitude > 0)
            _rigidbody.AddForce(-_rigidbody.velocity.normalized * (inputManager.Airbrake() * brakeMultiplier), ForceMode.Acceleration);

        var rotationalSpeed = thrust * thrustMultiplier;
        _rigidbody.AddTorque(transform.rotation * Vector3.right * rotationalSpeed * (inputManager.Pitch() * pitchMultiplier), ForceMode.Acceleration);
        _rigidbody.AddTorque(transform.rotation * Vector3.up * rotationalSpeed * (inputManager.Yaw() * yawMultiplier), ForceMode.Acceleration);
        _rigidbody.AddTorque(transform.rotation * Vector3.forward * rotationalSpeed * (inputManager.Roll() * rollMultiplier), ForceMode.Acceleration);
        
        if(frontRadar == null)
            return;
        
        var targetShip = frontRadar.spaceships.Count > 0 ? frontRadar.spaceships[0] : null;

        if (crosshair == null)
            return;
        
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
                
                var laser = World.Instance.Instantiate(laserPrefab);
                laser.transform.position = pistols[i].position;
                laser.Fire(this, target, bulletSpeed);
            }
            
            yield return new WaitForSeconds(1 / fireRate);
        }
    }

    private void CheckEngineSound()
    {
        if (engineSound == null)
            return;
        
        var overdrive = inputManager.Overdrive();
        var thrust = inputManager.Thrust();

        if (overdrive != _overdrive)
        {
            _soundTween?.Kill();
            
            if (overdrive)
            {
                boosterActivation.Play();
                
                boosterSound.Play();
                
                boosterVfx.Play();
                
                _soundTween = DOTween.To(x =>
                {
                    _overdriveTweenValue = x;
                    
                    boosterSound.volume = x * BoosterMaxVolume;

                    var currentEngineSound = inputManager.Thrust();
                    UpdateEngineSound(currentEngineSound + (1.5f - currentEngineSound) * x);
                }, _overdriveTweenValue, 1, 0.75f);
            }
            else
            {
                boosterDeactivation.Play();
                
                boosterVfx.Stop();
                
                _soundTween = DOTween.To(x =>
                {
                    _overdriveTweenValue = x;
                    
                    boosterSound.volume = x * BoosterMaxVolume;

                    var currentEngineSound = inputManager.Thrust();
                    UpdateEngineSound(currentEngineSound + (1.5f - currentEngineSound) * x);
                }, _overdriveTweenValue, 0, 0.75f).OnComplete(() => boosterSound.Stop());
            }
        }
        else UpdateEngineSound(thrust);

        _overdrive = overdrive;
    }

    private void UpdateEngineSound(float soundValue)
    {
        engineSound.volume = soundValue * 0.075f + 0.05f;
        engineSound.pitch = soundValue * 0.7f + 0.9f;
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
        var deathVfx = World.Instance.Instantiate(this.deathVfx);
        deathVfx.transform.position = transform.position;
        Destroy(deathVfx, 5f);
        Destroy(gameObject);
    }
}
