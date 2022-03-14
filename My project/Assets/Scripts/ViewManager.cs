using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewManager : MonoBehaviour
{

    [SerializeField] private Camera _camera;
    private Transform _camTransform;

    [SerializeField] private GameObject[] _actViews;

    private int _currentView = 1;

    [SerializeField] private GameObject _leftButton;
    [SerializeField] private GameObject _rightButton;
    [SerializeField] private TMP_Text _debugText;



    private void Awake()
    {
        _camTransform = _camera.transform;
        LookAtView();
    }


    void Start()
    {
                
    }


    public void UpdateDebug(string text)
    {
        _debugText.text = text;
    }


    public void LookLeft()
    {
        _currentView--;

        if (_currentView == 0)
        {
            _leftButton.SetActive(false);
        }
        _rightButton.SetActive(true);

        LookAtView();

    }


    public void LookRight()
    {
        _currentView++;

        if (_currentView == _actViews.Length - 1)
        {
            _rightButton.SetActive(false);
        }
        _leftButton.SetActive(true);

        LookAtView();
    }


    private void LookAtView()
    {
        var curPos = _actViews[_currentView].transform.position;
        _camTransform.position = curPos;
    }

}
