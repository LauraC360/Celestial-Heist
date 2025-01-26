using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class AIShip : InputManager
{
    [SerializeField] private float minDistanceThreshold;

    public bool ReachedDestination => _reachedDestination;

    private Vector3 _targetPosition;
    private bool _reachedDestination = true;

    private float _thrust = 0.65f;
    private float _roll = 0;
    private float _yaw = 0;
    private float _pitch = 0;
    private const float DegreeEpsilon = 5f;

    protected virtual void Start()
    {
        StartCoroutine(Movement());
    }
    public void SetTargetPosition(Vector3 position)
    {
        _reachedDestination = false;
        _targetPosition = position;
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            var diff = _targetPosition - transform.position;
            _thrust = diff.magnitude < minDistanceThreshold ? 0f : 0.65f;

            var targetRotation = Quaternion.LookRotation(diff, Vector3.up);

            var angleDiff = targetRotation.eulerAngles - transform.eulerAngles;
            _pitch = GetInput(angleDiff.x);
            _yaw = GetInput(angleDiff.y);
            _roll = GetInput(angleDiff.z);
            if (diff.magnitude < minDistanceThreshold)
            {
                _reachedDestination = true;
                yield return new WaitUntil(() => !_reachedDestination);
            }
        }
    }
    private float GetInput(float diff)
    {
        if (diff > 180)
            diff -= 360;

        if (diff < -180)
            diff += 360;

        if (diff > DegreeEpsilon)
            return Mathf.Abs(diff) / 180 * 0.75f;

        if (diff < -DegreeEpsilon)
            return -Mathf.Abs(diff) / 180 * 0.75f;
        return 0;
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