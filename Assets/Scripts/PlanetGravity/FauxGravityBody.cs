using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// https://www.youtube.com/watch?v=gHeQ8Hr92P4
public class FauxGravityBody : MonoBehaviour {

    public FauxGravityAttractor attractor;
    private Transform myTransform;
    Rigidbody myRigidbody;
    PlanetGenerator planetGenerator;
    Queue<Tuple<GameObject, float>> planetQueue;

    GameObject closestPlanet;

    const float planetGravityDistance = 10f;

    bool isInPlanetGravity = false;

    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        myTransform = transform;
        planetGenerator = FindObjectOfType<PlanetGenerator>();
        planetQueue = planetGenerator.planetQueue;

        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        myRigidbody.useGravity = true;
    }

    void Update() {
        closestPlanet = findClosestPlanet();
        Debug.Log("Closest planet: " + closestPlanet);
        Debug.Log("Is in planet gravity: " + isInPlanetGravity);
        if (attractor != null)
            Debug.Log("Attractor: " + attractor.gravity);
        else Debug.Log("Attractor: null");

        if (closestPlanet != null) {
            isInPlanetGravity = true;
            attractor = closestPlanet.GetComponent<FauxGravityAttractor>();
            myRigidbody.useGravity = false;
        } else {
            isInPlanetGravity = false;
            attractor = null;
            myRigidbody.useGravity = true;
        }

        if (isInPlanetGravity && attractor != null) {
            attractor.Attract(myTransform);
        }
    }

    GameObject findClosestPlanet() {
        GameObject closestPlanet = null;
        float closestDistance = Mathf.Infinity;

        foreach (Tuple<GameObject, float> planet in planetQueue) {
            float distance = Vector3.Distance(myTransform.position, planet.Item1.transform.position) - planet.Item2;
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPlanet = planet.Item1;
            }
        }

        if (closestDistance > planetGravityDistance)
            return null;

        return closestPlanet;
    }
}
