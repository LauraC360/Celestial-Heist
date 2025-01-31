using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float InteractRange = 20f;
    public Camera PlayerCamera;
    public LayerMask interactableLayerMask; // Layer mask for interactable objects
    private Collectible currentCollectable;

    void Start()
    {
        if (interactableLayerMask == 0)
        {
            interactableLayerMask = LayerMask.GetMask("Collectible");
        }

        if (PlayerCamera == null)
        {
            PlayerCamera = gameObject.GetComponent<Camera>();
        }
    }

    void Update()
    {
        Ray ray = PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * InteractRange, Color.red);

        if (Physics.Raycast(ray, out hit, InteractRange, interactableLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            Collectible collectible = hitObject.GetComponent<Collectible>();
            Debug.Log("HitObject: " + hitObject.name + "\tCollectible: " + collectible + "\tCurrent Collectible: " + currentCollectable);
            if (collectible != null && collectible != currentCollectable)
            {
                currentCollectable = collectible;
                // interactionText.SetActive(true);
                // interactionText = currentCollectable.GetInteractionText();
            }
        }
        else
        {
            Debug.Log("No collectible");
            currentCollectable = null;
            // interactionText.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentCollectable?.Collect();
        }
    }
}
