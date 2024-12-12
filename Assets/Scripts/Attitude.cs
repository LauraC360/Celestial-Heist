using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Attitude : MonoBehaviour
{
    [SerializeField] private Transform ship;
    [SerializeField] private Transform rollDisplay;
    [SerializeField] private Transform pitchDisplay;

    private const float DotMaxHeight = 4;

    private const float UpdateInterval = 0.15f;

    private void Start()
    {
        StartCoroutine(AttitudeUpdate());
    }

    private IEnumerator AttitudeUpdate()
    {
        while (true)
        {
            var euler = ship.eulerAngles;
            rollDisplay.DOLocalRotate( new Vector3(0, -euler.z, 0), UpdateInterval);
            Debug.Log(euler);
            
            var height = 0f;
            if (euler.x > 180)
            {
                if (euler.x < 270)
                    euler.x = 270;
                
                height = (euler.x - 360) / 90;
            }
            else
            {
                if (euler.x > 90)
                    euler.x = 90;
            
                height = euler.x / 90;
            }

            pitchDisplay.DOLocalMoveZ(height * DotMaxHeight, UpdateInterval);

            yield return new WaitForSeconds(UpdateInterval);
        }
    }
}
