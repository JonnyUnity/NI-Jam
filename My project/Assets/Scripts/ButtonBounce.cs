using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBounce : MonoBehaviour
{

    [SerializeField] private bool _isLeftButton;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;    
    }

    // Update is called once per frame
    void Update()
    {
        float newX = 0.1f * Mathf.Sin(Time.time * 3f);
        if (_isLeftButton)
        {
            newX = _transform.position.x - newX;
        }
        else
        {
            newX = _transform.position.x + newX;
        }

        _transform.position = new Vector3(newX, _transform.position.y, _transform.position.z);
    }
}
