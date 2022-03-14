using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour
{

    [SerializeField] private int _objectID;

    private void Awake()
    {
        GameEvents.OnActivateObject += Activate;
        Debug.Log("hello there!");
    }


    private void OnDisable()
    {
        GameEvents.OnActivateObject -= Activate;
    }


    private void Activate(int objectID)
    {
        if (objectID == _objectID)
        {
            Debug.Log($"Object {objectID} reporting for duty!");
        }
        
    }
}
