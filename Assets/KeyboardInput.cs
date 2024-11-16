using System;
using UnityEngine;

[Serializable]
public class KeyboardInput : InputManager
{
    [SerializeField] private KeyCode rollLeft;
    [SerializeField] private KeyCode rollRight;
    [SerializeField] private KeyCode yawLeft;
    [SerializeField] private KeyCode yawRight;
    [SerializeField] private KeyCode pitchUp;
    [SerializeField] private KeyCode pitchDown;
    [SerializeField] private KeyCode thrust;
    [SerializeField] private KeyCode brake;
    
    public override float Roll()
    {
        return (Input.GetKey(rollLeft) ? -1 : 0) + (Input.GetKey(rollRight) ? 1 : 0);
    }

    public override float Yaw()
    {
        return (Input.GetKey(yawLeft) ? -1 : 0) + (Input.GetKey(yawRight) ? 1 : 0);
    }

    public override float Pitch()
    {
        return (Input.GetKey(pitchDown) ? -1 : 0) + (Input.GetKey(pitchUp) ? 1 : 0);
    }

    public override float Thrust()
    {
        return Input.GetKey(thrust) ? 1 : 0;
    }

    public override float Airbrake()
    {
        return Input.GetKey(brake) ? 1 : 0;
    }
}
