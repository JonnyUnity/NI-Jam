using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingPerson : MonoBehaviour
{

    [SerializeField] private float _walkSpeed;
    [SerializeField] private Transform _startTransform;
    [SerializeField] private Transform _endTransform;

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private Transform _transform;

    private bool _stopped;
    private bool _alreadyInteracted;

    private void Awake()
    {
        _transform = transform;
        _startPosition = _startTransform.position;
        _endPosition = _endTransform.position;
        _transform.position = _startPosition;
    }

    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        if (!_stopped && !_alreadyInteracted)
        {
            _stopped = true;
            _alreadyInteracted = true;
        }
        else if (_stopped)
        {
            _stopped = false;
        }


        //if (_alreadyInteracted)
        //    return;

        //_stopped = !_stopped;
        //_alreadyInteracted = true;
        Debug.Log("Person clicked!");
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopped)
            return;

        _transform.localPosition = Vector3.MoveTowards(_transform.localPosition, _endPosition, Time.deltaTime * _walkSpeed);
        
        if (_transform.localPosition == _endPosition)
        {
            Destroy(gameObject);
        }
        
    }
}
