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

    private bool _previousLeftButtonVisible;
    private bool _previousRightButtonVisible;

    private int _tutorialStep = 1;

    private void Awake()
    {
        _camTransform = _camera.transform;
        LookAtView();
    }

    private void OnEnable()
    {
        GameEvents.OnInteractionStart += InteractionStarted;
        GameEvents.OnInteractionEnd += ShowNavButtons;

        GameEvents.OnDialogueStarted += HideNavButtons;
        GameEvents.OnDialogueEnded  += ShowNavButtons;

        GameEvents.OnOpenDesktop += HideNavButtons;
        GameEvents.OnCloseDesktop += ShowNavButtons;

    }


    private void OnDisable()
    {
        GameEvents.OnInteractionStart -= InteractionStarted;
        GameEvents.OnInteractionEnd += ShowNavButtons;

        GameEvents.OnDialogueStarted -= HideNavButtons;
        GameEvents.OnDialogueEnded -= ShowNavButtons;

        GameEvents.OnOpenDesktop -= HideNavButtons;
        GameEvents.OnCloseDesktop -= ShowNavButtons;

    }

    
    private void InteractionStarted(int obj)
    {
        HideNavButtons();
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
         _leftButton.SetActive(_previousLeftButtonVisible);
        _rightButton.SetActive(_previousRightButtonVisible);
    }

    void Start()
    {
                
    }


    public void UpdateDebug(string text)
    {
        _debugText.text = text;
    }


    public void ToggleNavButtons(bool toggle)
    {
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
        _leftButton.SetActive(true);
        _rightButton.SetActive(false);
    }

    public void EnableRightArrow()
    {
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

        switch (_tutorialStep)
        {
            case 1:

                _rightButton.SetActive(false);
                GameEvents.ProgressStory();
                _tutorialStep++;
                break;

            case 2:

                _rightButton.SetActive(false);
                GameEvents.ProgressStory();
                _tutorialStep++;
                Debug.Log("Tutorial step 2");
                break;

            case 3:

                _rightButton.SetActive(false);
                _tutorialStep++;
                Debug.Log("Tutorial step 3");
                GameEvents.ProgressStory();
                break;

            case 4:

                _rightButton.SetActive(false);
                _tutorialStep++;
                Debug.Log("Step 4");
                GameEvents.ProgressStory();
                break;

            case 5:

                _tutorialStep++;
                GameEvents.ProgressStory();
                break;

        }

    }

}

public enum DeskView
{
    LEFT,
    MIDDLE,
    RIGHT
}
