using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravityStrength = 9.81f;
    public float activationDistance = 10f; // Distance within which gravity is activated
    private Transform player;
    private Rigidbody playerRigidbody;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerRigidbody = other.GetComponent<Rigidbody>();
            playerRigidbody.useGravity = false; // Disable default gravity
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigidbody.useGravity = true; // Re-enable default gravity
            player = null;
            playerRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (player != null && playerRigidbody != null)
        {
            float distanceToPlanet = Vector3.Distance(transform.position, player.position);
            if (distanceToPlanet <= activationDistance)
            {
                Vector3 directionToPlanet = (transform.position - player.position).normalized;
                playerRigidbody.AddForce(directionToPlanet * gravityStrength, ForceMode.Acceleration);

                // Rotate the player to align with the planet's surface
                Quaternion targetRotation = Quaternion.FromToRotation(player.up, -directionToPlanet) * player.rotation;
                playerRigidbody.MoveRotation(Quaternion.Slerp(player.rotation, targetRotation, Time.fixedDeltaTime * 5f));
            }
        }
    }
}