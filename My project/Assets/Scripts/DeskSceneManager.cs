using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskSceneManager : Singleton<DeskSceneManager>
{

    //[SerializeField] private TutorialHandler _tutorial;

    [SerializeField] private ViewManager _viewManager;

    [SerializeField] private Phone _phoneObject;
    [SerializeField] private GameObject _pillsObject;

    //[SerializeField] private MedsAlarm _medsAlarmObject;
    [SerializeField] private Computer _computerObject;
    [SerializeField] private Plant _plantObject; 

    [SerializeField] private CoWorkerInteraction _bobInteraction;
    [SerializeField] private CoWorkerInteraction _gossipInteraction;

    private PlayerState State;
    private float _timeToEvent;
    private bool _timerOn;
    private int _interactionID;
    private int _tutorialID;
    private Action _timedEvent;

    // Plant
    [SerializeField] private int _minTimeBetweenWaterings;
    private int _timeSinceLastWater;


    private static readonly Dictionary<int, int> _actsActionCount = new Dictionary<int, int>()
    {
        {0, 13 },
        {1, 10 },
        {2, 10 },
        {3, 10 }
    };
    private int _actNumber;
    private int _storyBeat;


    private void Awake()
    {
        GameEvents.OnActStart += StartAct;
        GameEvents.OnInteractionStart += UpdateStateToBusy;
        GameEvents.OnInteractionEnd += UpdateStateToIdle;
        GameEvents.OnWaterPlant += WaterPlant;
        GameEvents.OnStoryActionPerformed += NextStoryBeat;

        State = PlayerState.IDLE;
    }



    private void OnDisable()
    {
        GameEvents.OnActStart -= StartAct;
        GameEvents.OnInteractionStart -= UpdateStateToBusy;
        GameEvents.OnInteractionEnd -= UpdateStateToIdle;
        GameEvents.OnWaterPlant -= WaterPlant;
        GameEvents.OnStoryActionPerformed -= NextStoryBeat;
    }

    private void UpdateStateToIdle()
    {
        State = PlayerState.IDLE;
    }

    private void UpdateStateToBusy(int obj)
    {
        State = PlayerState.BUSY;
    }

    private void Start()
    {
        //SetupStoryBeat();
    }

    private void Update()
    {
        if (!_timerOn)
            return;

        if (State == PlayerState.BUSY)
            return;

        _timeToEvent -= Time.deltaTime;

        if (_timeToEvent <= 0f)
        {
            Debug.Log("Time's up! run event!");
            if (_timedEvent != null)
            {
                _timedEvent();
            }
            _timerOn = false;
        }



    }

    public int GetGameTime()
    {
        return (_actNumber * 100) + _storyBeat;
    }


    private void StartAct(int actNumber)
    {
        _actNumber = actNumber;
        _storyBeat = 1;
        SetupStoryBeat();
    }


    private void NextStoryBeat()
    {

        _storyBeat++;
        _timeSinceLastWater++;

        if (_actsActionCount[_actNumber] == _storyBeat)
        {
            if (_actNumber == 0) // tutorial
            {
                _actNumber++;
                _storyBeat = 1;
                SetupStoryBeat();
            }
            else
            {
                GameEvents.ActOver();
            } 

        }
        else
        {
            Debug.Log("I'm the next story beat! " + _storyBeat);
            SetupStoryBeat();
        }

    }


    private void SetupStoryBeat()
    {
        switch (_actNumber)
        {
            case 0:

                SetupTutorialBeat();
                break;

            case 1:

                SetupAct1StoryBeat();
                break;

            case 2:

                SetupAct2StoryBeat();
                break;
            case 3:

                SetupAct3StoryBeat();
                break;

            default:

                Debug.Log($"Unknown act! {_actNumber}, story beat:{_storyBeat}");
                break;
        }  
        
        if (_timeSinceLastWater == _minTimeBetweenWaterings)
        {
            _plantObject.UpdatePlantStatus(false);
        }
        else if (_timeSinceLastWater >= _minTimeBetweenWaterings + 2)
        {
            _plantObject.UpdatePlantStatus(true);
        }

    }

    private delegate void MyDelegate(int interactionID);



    private void SetupTutorialBeat()
    {
        Debug.Log($"setting up tutorial beat {_storyBeat} in act {_actNumber}");

        switch (_storyBeat)
        {
            case 1:  // TUTORIAL: WELCOME

                SetupTutorial(1, 1f);

                //_tutorial.StartTutorial(1);
                //_phoneObject.ReceivePhoneCall(1);
                //GameEvents.MedsAlarmGoesOff(1);
                //GameEvents.PhoneRings(1);
                //StartCoroutine(GameManager.Instance.BobVisit1());
                // Invoke("BobVisit1", 4f);
                break;

            case 2: // TUTORIAL: PHONE


                _phoneObject.PhoneTutorial();
                //TutorialHandler.Instance.StartTutorial(2);
                SetupTutorial(2, 1f);
                _phoneObject.ReceivePhoneCall(1);

                break;

            case 3: // TUTORIAL: GO TO WATER COOLER

                SetupTutorial(3, 1f);
                //_viewManager.EnableLeftArrow();
                break;

            case 4: // TUTORIAL: GO TO WATER COOLER 2

                _viewManager.EnableLeftArrow();
                break;

            case 5: // TUTORIAL: GO TO WATER COOLER 3

                SetupTutorial(4, 1f);
                break;

            case 6: // TUTORIAL: CLICKED ON WATER COOLER

                SetupTutorial(5, 0f);
                _viewManager.EnableRightArrow();
                break;

            case 7: // TUTORIAL: WATER PLANT

                _viewManager.ToggleNavButtons(false);
                _plantObject.DoTutorial();
                SetupTutorial(6, 1f);
                break;

            case 8: // TUTORIAL: OFFICE GOSSIP

                _viewManager.EnableLeftArrow();
                SetupTutorial(7, 1f);
                break;

            case 9: // TUTORIAL: REPLY TO EMAIL

                SetupTutorial(8, 1f);
                _gossipInteraction.DoTutorial(1);

                break;

            case 10:

                SetupTutorial(9, 1f);
                _viewManager.EnableRightArrow();
                break;

            case 11:

                _viewManager.ToggleNavButtons(false);
                _computerObject.EnableComputer();
                SetupTutorial(10, 1f);
                break;

            case 12:

                // computer tutorial!
                
                break;

            
            default:
                Debug.Log("Out of story beats! Shouldn't be here!");
                break;

        }
    }

    private void SetupAct1StoryBeat()
    {
        Debug.Log($"setting up story beat {_storyBeat} in act {_actNumber}");

        switch (_storyBeat)
        {
            case 1:

                _viewManager.ToggleNavButtons(true);
                SetupTimedEvent(2f, BobVisit1);
                break;

            case 2:


                //_phoneObject.PhoneTutorial();
                //TutorialHandler.Instance.StartTutorial(2);
                //SetupTutorial(2, 1f);
                //_phoneObject.ReceivePhoneCall(1);

                break;

            case 3:

                break;

            case 4:


                break;

            case 5:


                break;

            case 6:

                break;

            case 7:


                break;

            case 8:


                break;

            case 9:

                break;

            case 10:

                break;

            default:
                Debug.Log("Out of story beats! Shouldn't be here!");
                break;

        }
    }

    private void SetupAct2StoryBeat()
    {
        Debug.Log($"setting up story beat {_storyBeat} in act {_actNumber}");

        switch (_storyBeat)
        {
            case 1:

                //_phoneObject.ReceivePhoneCall(1);
                //GameEvents.PhoneRings(1);
                break;

            case 2:

                //StartCoroutine(GameManager.Instance.BobVisit1());
                break;

            case 3:

                //GameEvents.MedsAlarmGoesOff(1);

                break;

            case 4:

                break;

            case 5:

                break;

            case 6:

                break;

            case 7:

                break;

            case 8:

                break;

            case 9:

                break;

            case 10:

                break;

            default:
                Debug.Log("Out of story beats! Shouldn't be here!");
                break;

        }
    }

    private void SetupAct3StoryBeat()
    {
        Debug.Log($"setting up story beat {_storyBeat} in act {_actNumber}");

        switch (_storyBeat)
        {
            case 1:

                //_phoneObject.ReceivePhoneCall(1);
                GameEvents.PhoneRings(1);
                break;

            case 2:

               // GameManager.Instance.BobVisit1();
                break;

            case 3:

                GameEvents.MedsAlarmGoesOff(1);

                break;

            case 4:

                break;

            case 5:

                break;

            case 6:

                break;

            case 7:

                break;

            case 8:

                break;

            case 9:

                break;

            case 10:

                break;

            default:
                Debug.Log("Out of story beats! Shouldn't be here!");
                break;

        }
    }


    private void SetupTimedEvent(float timer, Action action)
    {
        Debug.Log($"Set up timed event: timer={timer}, Action={action}");
        _timeToEvent = timer;
        _timedEvent = action;
        _timerOn = true;
    }

    private void SetupTutorial(int tutorialID, float timer)
    {
        Debug.Log($"Set up timed event: timer={timer}");
        _tutorialID = tutorialID;
        _timeToEvent = timer;
        _timedEvent = DoTutorial;
        _timerOn = true;
    }

    private void DoTutorial()
    {
        TutorialHandler.Instance.StartTutorial(_tutorialID);
    }

    
    private void WaterPlant()
    {
        PlayerChoices.Instance.ChangePlayerValue("WateredCount", 1);
        _timeSinceLastWater = 0;
    }


    public void BobVisit1()
    {

        if (PlayerChoices.Instance.WatchedGameLastNight)
        {
            _interactionID = 1;
        }
        else
        {
            _interactionID = 2;
        }
        _bobInteraction.DoVisit(_interactionID);
    }

}

public enum PlayerState
{
    IDLE,
    BUSY
}
