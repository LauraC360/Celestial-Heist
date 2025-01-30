using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingShip : InputManager
{
    [SerializeField] private float minDistanceThreshold = 50f;
    [SerializeField] private List<RacingPoint> racingPoints;
    private int _currentCheckpointIndex = 0;

    private Vector3 _targetPosition;

    private float _thrust = 6f;
    private const float RotationSpeed = 4.0f;

    protected virtual void Start()
    {
        if (racingPoints.Count > 0)
        {
            _targetPosition = racingPoints[_currentCheckpointIndex].transform.position;
        }
    }

    private void UpdateTarget()
    {
        _currentCheckpointIndex++;
        if (_currentCheckpointIndex >= racingPoints.Count)
        {
            _currentCheckpointIndex = 1;
        }
        _targetPosition = racingPoints[_currentCheckpointIndex].transform.position;
    }

    private void Update()
    {
        Vector3 diff = _targetPosition - transform.position;
        _thrust = 0.65f;

        Quaternion targetRotation = Quaternion.LookRotation(diff, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed);

        if (diff.magnitude < minDistanceThreshold)
        {
            UpdateTarget();
        }
    }

    public override float Roll() => 0;
    public override float Yaw() => 0;
    public override float Pitch() => 0;
    public override float Thrust() => _thrust;
    public override float Airbrake() => 0;
    public override bool Fire() => false;
}
