using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ViewManager : MonoBehaviour
{

    [SerializeField] private Camera _camera;
    private Transform _camTransform;

    [SerializeField] private GameObject[] _actViews;

    private int _currentView = 1;

    [SerializeField] private GameObject _leftButton;
    [SerializeField] private GameObject _rightButton;
    [SerializeField] private TMP_Text _debugText;

    private Vector3 _leftbuttonInitPosition;
    private Vector3 _rightButtonInitPosition;
    private bool _previousLeftButtonVisible;
    private bool _previousRightButtonVisible;

    private bool _isInTutorial;
    private int _tutorialStep = 1;

    private void Awake()
    {
        _camTransform = _camera.transform;
        _leftbuttonInitPosition = _leftButton.transform.position;
        _rightButtonInitPosition = _rightButton.transform.position;
        LookAtView();
    }

    private void OnEnable()
    {
        GameEvents.OnInteractionStart += InteractionStarted;
        GameEvents.OnInteractionEnd += ShowNavButtons;

        //GameEvents.OnDialogueStarted += HideNavButtons;
        //GameEvents.OnDialogueEnded  += ShowNavButtons;

        GameEvents.OnOpenDesktop += HideNavButtons;
        GameEvents.OnCloseDesktop += ShowNavButtons;

        GameEvents.OnTutorialEnded += EndTutorial;

    }



    private void OnDisable()
    {
        GameEvents.OnInteractionStart -= InteractionStarted;
        GameEvents.OnInteractionEnd -= ShowNavButtons;

        //GameEvents.OnDialogueStarted -= HideNavButtons;
        //GameEvents.OnDialogueEnded -= ShowNavButtons;

        GameEvents.OnOpenDesktop -= HideNavButtons;
        GameEvents.OnCloseDesktop -= ShowNavButtons;

        GameEvents.OnTutorialEnded -= EndTutorial;
    }

    
    private void InteractionStarted()
    {
        HideNavButtons();
    }

    private void EndTutorial()
    {
        _isInTutorial = false;
    }

    private void HideNavButtons()
    {
        _previousLeftButtonVisible = _leftButton.activeInHierarchy;
        _previousRightButtonVisible = _rightButton.activeInHierarchy;

        _leftButton.SetActive(false);
        _rightButton.SetActive(false);

    }


    private void ShowNavButtons()
    {

        if (_isInTutorial)
        {

             switch (_tutorialStep)
            {
                case 1:

                    _rightButton.SetActive(false);
                    break;

                case 2:

                    //_rightButton.SetActive(false);
                    ToggleNavButtons(false);
                    break;

                case 3:

                    ToggleNavButtons(false);
                    break;

                case 4:

                    ToggleNavButtons(false);
                    break;

                case 5:

                    //ToggleNavButtons(false);
                    //_leftButton.SetActive(true);
                    break;
                case 6:

                    _leftButton.SetActive(true);
                    break;

            }

        }
        else
        {

            _leftButton.transform.position = _leftbuttonInitPosition;
            _rightButton.transform.position = _rightButtonInitPosition;

            switch (_currentView)
            {
                case (int)DeskView.LEFT:

                    _leftButton.SetActive(false);
                    _rightButton.SetActive(true);
                    break;

                case (int)DeskView.MIDDLE:

                    _leftButton.SetActive(true);
                    _rightButton.SetActive(true);
                    break;

                case (int)DeskView.RIGHT:

                    _leftButton.SetActive(true);
                    _rightButton.SetActive(false);
                    break;
            }
        }

        //_leftButton.SetActive(_previousLeftButtonVisible);
        //_rightButton.SetActive(_previousRightButtonVisible);
    }


    public void EnableTutorial()
    {
        _isInTutorial = true;
    }


    //private void AlarmStopped(bool isRight)
    //{
    //    if (isRight)
    //    {
    //        _currentView = 1;
    //        LookAtView();
    //        ToggleNavButtons(false);
    //    }
    //}


    public void UpdateDebug(string text)
    {
        _debugText.text = text;
    }


    public void ToggleNavButtons(bool toggle)
    {
        _leftButton.transform.position = _leftbuttonInitPosition;
        _rightButton.transform.position = _rightButtonInitPosition;
        _leftButton.SetActive(toggle);
        _rightButton.SetActive(toggle);
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

    public void EnableLeftArrow()
    {
        _leftButton.transform.position = _leftbuttonInitPosition;
        _leftButton.SetActive(true);
        _rightButton.SetActive(false);
    }

    public void EnableRightArrow()
    {
        _rightButton.transform.position = _rightButtonInitPosition;
        _rightButton.SetActive(true);

    }

    
    public void GoToView(DeskView view)
    {
        _currentView = (int)view;

        if (_currentView == 0)
        {
            _leftButton.SetActive(false);
        }
        else
        {
            _leftButton.SetActive(true);
        }
        if (_currentView == _actViews.Length - 1)
        {
            _rightButton.SetActive(false);
        }
        else
        {
            _rightButton.SetActive(true);
        }

        LookAtView();
    }

    private void LookAtView()
    {
        var curPos = _actViews[_currentView].transform.position;
        _camTransform.position = curPos;

        if (_isInTutorial)
        {

            switch (_tutorialStep)
            {
                case 1:

                    _rightButton.SetActive(false);
                    GameEvents.NextTutorialStep();
                    break;

                case 2:

                    //_rightButton.SetActive(false);
                    ToggleNavButtons(false);
                    GameEvents.NextTutorialStep();
                    Debug.Log("VIEW step 2");
                    break;

                case 3:

                    ToggleNavButtons(false);
                    Debug.Log("VIEW step 3");
                    GameEvents.NextTutorialStep();
                    break;

                case 4:

                    ToggleNavButtons(false);
                    Debug.Log("VIEW Step 4");
                    GameEvents.NextTutorialStep();                    
                    break;

                case 5:

                    //ToggleNavButtons(false);
                    _leftButton.SetActive(true);
                    _previousLeftButtonVisible = true;
                    _previousRightButtonVisible = false;
                    Debug.Log("VIEW Step 5");

                    GameEvents.NextTutorialStep();
                    break;

                case 6:

                    _leftButton.SetActive(true);
                    Debug.Log("VIEW Step 6");
                    _isInTutorial = false;
                    break;

            }

            _tutorialStep++;
        }

    }

}

public enum DeskView
{
    LEFT,
    MIDDLE,
    RIGHT
}
