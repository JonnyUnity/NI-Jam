using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionTimer : MonoBehaviour
{

    private Action _action;
    private float _timer;
    private bool _timerOn;

    public void StartTimer(Action action, float timer)
    {
        _action = action;
        _timer = timer;
        _timerOn = true;
    }

    


    // Update is called once per frame
    public void Update()
    {

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _timerOn = false;
            Debug.Log("Time's up! run event!");
            _action();
        }

    }
}
