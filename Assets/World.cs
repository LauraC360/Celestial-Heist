using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    private List<Rigidbody> childrenRigidbodies;
    private List<Transform> childrenTransforms;

    private void Awake()
    {
        Instance = this;
        
        childrenRigidbodies = new List<Rigidbody>();
        childrenTransforms = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            
            var rigidbody = child.GetComponent<Rigidbody>();
            if (rigidbody != null)
                childrenRigidbodies.Add(rigidbody);
            else childrenTransforms.Add(child);
        }
    }

    public T Instantiate<T>(T prefab) where T: Component
    {
        var obj = GameObject.Instantiate(prefab);
        obj.transform.SetParent(transform);
        
        var rigidbody = obj.GetComponent<Rigidbody>();
        if(rigidbody != null)
            childrenRigidbodies.Add(rigidbody);
        else childrenTransforms.Add(obj.transform);

        return obj;
    }
    
    public GameObject Instantiate(GameObject prefab)
    {
        var obj = GameObject.Instantiate(prefab);
        obj.transform.SetParent(transform);
        
        var rigidbody = obj.GetComponent<Rigidbody>();
        if(rigidbody != null)
            childrenRigidbodies.Add(rigidbody);
        else childrenTransforms.Add(obj.transform);

        return obj;
    }

    public void Move(Vector3 delta)
    {
        for (int i = 0; i < childrenTransforms.Count; i++)
        {
            var child = childrenTransforms[i];
            if (child == null)
            {
                childrenTransforms.RemoveAt(i--);
                continue;
            }
            
            child.position += delta;
        }
        
        for (int i = 0; i < childrenRigidbodies.Count; i++)
        {
            var child = childrenRigidbodies[i];
            if (child == null)
            {
                childrenRigidbodies.RemoveAt(i--);
                continue;
            }
            
            child.position += delta;
        }
    }
}
