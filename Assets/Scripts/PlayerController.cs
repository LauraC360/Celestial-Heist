using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController CC;

    float Gravity = -3.5f;

    [SerializeField] float WalkingSpeed = 1f;

    Vector3 velocity;

    Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!CC.isGrounded)
        {
            velocity.y += Gravity * Time.deltaTime;

            CC.Move(velocity * Time.deltaTime);
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        CC.Move(move * WalkingSpeed * Time.deltaTime);
    }
}
