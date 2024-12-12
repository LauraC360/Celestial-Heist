using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpaceship : InputManager
{
    private float _thrust = 0.65f;
    private float _roll = 0;
    private float _yaw = 0;
    private float _pitch = 0;

    private void Start()
    {
        StartCoroutine(RandomMovement());
    }

    private IEnumerator RandomMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            _roll = Random.Range(-1f, 1f);
            _yaw = Random.Range(-1f, 1f);
            _pitch = Random.Range(-1f, 1f);
        }
    }

    public override float Roll()
    {
        return _roll;
    }

    public override float Yaw()
    {
        return _yaw;
    }

    public override float Pitch()
    {
        return _pitch;
    }

    public override float Thrust()
    {
        return _thrust;
    }

    public override float Airbrake()
    {
        return 0;
    }

    public override bool Fire()
    {
        return false;
    }
}
