using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Collectible : MonoBehaviour
{
    // void Update(){
    //     if (collectAction.action.triggered){
    //         Collect();
    //     }
    // }

    public void Initialize(int id){
        gameObject.name = $"Collectible_{id}";
        gameObject.tag = "Collectible";
        gameObject.layer = LayerMask.NameToLayer("Collectible");
        // add collider
        MeshCollider col = gameObject.AddComponent<MeshCollider>();
        col.convex = true;
        col.isTrigger = true;
        // gameObject.layer = LayerMask.NameToLayer("Collectible");

        // gameObject.AddComponent<XrInteractibleAffordnceStateProvider>();
        // gameObject.GetComponent<XrInteractibleAffordnceStateProvider>().IgnoreHover = true;

        XRGrabInteractable grab = gameObject.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        // smooth pos and rotation
        grab.trackPosition = true;
        grab.smoothPosition = true;
        grab.trackRotation = true;
        grab.smoothRotation = true;
        grab.interactionLayerMask = LayerMask.GetMask("Collectible");
        // set gravity off
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Collect(){
        // TODO add in inventory !
        Debug.Log("Collected");
        Destroy(gameObject);
    }

    
    void OnCollisionEnter(Collision collision){
        Debug.Log("Collision" + collision.gameObject.name);
    }
}
