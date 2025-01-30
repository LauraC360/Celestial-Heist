using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// https://www.youtube.com/watch?v=gHeQ8Hr92P4
public class FauxGravityAttractor : MonoBehaviour {

    [HideInInspector]
    public float gravity = -10f;

    public void Attract(Transform body) {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
