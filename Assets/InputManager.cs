using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    public abstract float Roll();
    public abstract float Yaw();
    public abstract float Pitch();
    public abstract float Thrust();
    public abstract float Airbrake();
}
