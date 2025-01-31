using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]

public class SC_RigidbodyWalker : MonoBehaviour
{
    float speed = 5.0f;
    bool canJump = true;
    float jumpHeight = 1.50f;
    public Camera playerCamera;
    float lookSpeed = 2.0f;
    float lookXLimit = 60.0f;

    bool grounded = false;
    Rigidbody r;
    Vector2 rotation = Vector2.zero;
    float maxVelocityChange = 10.0f;

    [SerializeField]
    InputActionProperty jumpAction;

    void Awake()
    {
        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = true;
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rotation.y = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Player and Camera rotation
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);

        Quaternion localRotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
        transform.rotation = transform.rotation * localRotation;
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 forwardDir = Vector3.Cross(transform.up, -playerCamera.transform.right).normalized;
            Vector3 rightDir = Vector3.Cross(transform.up, playerCamera.transform.forward).normalized;
            Vector3 targetVelocity = (forwardDir * Input.GetAxis("Vertical") + rightDir * Input.GetAxis("Horizontal")) * (speed / 4); // Halve the speed

            Vector3 velocity = transform.InverseTransformDirection(r.velocity);
            velocity.y = 0;
            velocity = transform.TransformDirection(velocity);
            Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange / 4, maxVelocityChange / 4); // Halve the max velocity change
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange / 4, maxVelocityChange / 4); // Halve the max velocity change
            velocityChange.y = 0;
            velocityChange = transform.TransformDirection(velocityChange);

            r.AddForce(velocityChange, ForceMode.VelocityChange);

            if (IsJumpPressed() && canJump) // to be changed to a controller button
            {
                r.AddForce(transform.up * (jumpHeight), ForceMode.VelocityChange); // Halve the jump height
            }
        }

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    bool IsJumpPressed()
    {
        // Debug.Log(jumpAction.action.WasPressedThisFrame());
        return jumpAction.action.WasPressedThisFrame();
    }
}
