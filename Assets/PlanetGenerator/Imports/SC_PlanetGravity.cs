using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PlanetGravity : MonoBehaviour
{
    PlanetGenerator planetGenerator;
    Queue<Tuple<GameObject, float>> planetQueue;
    GameObject closestPlanet;
    const float planetGravityDistance = 10f;

    public Transform planet;
    public bool alignToPlanet = true;

    float gravityConstant = 20f;
    Rigidbody r;
    Transform t;


    void Start()
    {
        r = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        planetGenerator = FindObjectOfType<PlanetGenerator>();
        planetQueue = planetGenerator.planetQueue;
    
    }

    void FixedUpdate()
    {
        closestPlanet = findClosestPlanet();



        if(planet != null){
            Vector3 toCenter = planet.position - transform.position;
            toCenter.Normalize();


            r.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);

            if (alignToPlanet)
            {
                Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
                q = q * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, q, 1);
            }
        }
    }

    GameObject findClosestPlanet() {
        GameObject closestPlanet = null;
        float closestDistance = Mathf.Infinity;

        foreach (Tuple<GameObject, float> planet in planetQueue) {
            float distance = Vector3.Distance(t.position, planet.Item1.transform.position) - planet.Item2;
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPlanet = planet.Item1;
            }
        }

        if (closestDistance > planetGravityDistance)
            return null;

        planet = closestPlanet.transform;
        return closestPlanet;
    }
}