using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskSceneManager : Singleton<DeskSceneManager>
{

    [SerializeField] private TutorialHandler _tutorialHandler;
    [SerializeField] private bool _skipTutorial;
    [SerializeField] private TestDebugger _debugger;

    //[SerializeField] private TutorialHandler _tutorial;
    [SerializeField] private TMPro.TMP_Text _dayHeadingText;
    [SerializeField] private Animator _animator;

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private ViewManager _viewManager;

    [SerializeField] private Phone _phoneObject;
    [SerializeField] private GameObject _pillsObject;
    [SerializeField] private Watercooler _waterCoolerObject;
    [SerializeField] private Cup _cupObject;

    [SerializeField] private Computer _computerObject;
    [SerializeField] private Plant _plantObject; 

    [SerializeField] private CoWorkerInteraction _bobInteraction;
    [SerializeField] private CoWorkerInteraction _simonGossipInteraction;
    [SerializeField] private CoWorkerInteraction _margeGossipInteraction;
    [SerializeField] private CoWorkerInteraction _susanGossipInteraction;
    [SerializeField] private CoWorkerInteraction _mattGossipInteraction;

    [SerializeField] private CoWorkerInteraction[] _coWorkers;
     private CoWorker _activeCoWorkerID;

    [SerializeField] private AudioClip[] _actBackgroundMusic;

   // private PlayerState State;
    private float _timeToEvent;
    private bool _timerOn;
    private int _interactionID;
    private int _tutorialID;
    private Action _timedEvent;

    private int _alarmID = 1;

    private bool _isInTutorial;

    // Plant
    [SerializeField] private int _minTimeBetweenWaterings;
    private int _timeSinceLastWater;

    private static readonly string[] _actTitles = { "Monday", "Wednesday", "Friday" };

    private static readonly int[] _actsActionCount = new int[]
    {
        0, 13, 10, 10
    };


    private int _actNumber;
    private int _storyBeat;

    public int GameTime => (_actNumber * 100) + _storyBeat;

    private void Awake()
    {
//#if UNITY_EDITOR
//        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
//#endif


        GameEvents.OnActStart += StartAct;
        GameEvents.OnTutorialEnded += EndTutorial;

        GameEvents.OnInteractionStart += UpdateStateToBusy;
        GameEvents.OnOpenDesktop += UpdateStateToBusy;
        GameEvents.OnCloseDesktop += UpdateStateToIdle;
        GameEvents.OnInteractionEnd += UpdateStateToIdle;
        GameEvents.OnPlantWatered += WaterPlant;
        GameEvents.OnStoryActionPerformed += NextStoryBeat;

        //State = PlayerState.IDLE;
        GameManager.Instance.UpdateStateToIdle();
    }



    private void OnDisable()
    {
        GameEvents.OnActStart -= StartAct;
        GameEvents.OnTutorialEnded -= EndTutorial;

        GameEvents.OnInteractionStart -= UpdateStateToBusy;
        GameEvents.OnOpenDesktop -= UpdateStateToBusy;
        GameEvents.OnCloseDesktop -= UpdateStateToIdle;
        GameEvents.OnInteractionEnd -= UpdateStateToIdle;
        GameEvents.OnPlantWatered -= WaterPlant;
        GameEvents.OnStoryActionPerformed -= NextStoryBeat;
    }

    private void UpdateStateToIdle()
    {
        //        State = PlayerState.IDLE;
        GameManager.Instance.UpdateStateToIdle();
    }

    private void UpdateStateToBusy()
    {
        //State = PlayerState.BUSY;
        GameManager.Instance.UpdateStateToBusy();
    }



    private void Update()
    {
        if (!_timerOn)
            return;

        if (GameManager.Instance.State == PlayerState.BUSY)
            return;

        _timeToEvent -= Time.deltaTime;

        if (_timeToEvent <= 0f)
        {
            _timerOn = false;
            Debug.Log("Time's up! run event!");
            _timedEvent?.Invoke();
        }

    }

    public int GetGameTime()
    {
        return (_actNumber * 100) + _storyBeat;
    }


    private void StartAct(int actNumber)
    {

        _dayHeadingText.text = _actTitles[actNumber - 1];

        StartCoroutine(FadeInCoroutine(actNumber)); 

    }


    private IEnumerator FadeInCoroutine(int actNumber)
    {
        _animator.SetTrigger("StartOfDay");
        yield return new WaitForSecondsRealtime(2f);

        _audioSource.Play();
        // begin tutorial...
        
        if (actNumber == 1)
        {

            if (_skipTutorial)
            {
                InitAct();

                _isInTutorial = false;
                _actNumber = 1;
                _storyBeat = 0;

                SetupStoryBeat();
            }
            else
            {
                _isInTutorial = true;
                _tutorialHandler.StartTutorial();
            }            
        }
        else
        {
            InitAct();
            _actNumber = actNumber;
            _storyBeat = 1;
            SetupStoryBeat();
        }
        

    }


    private void InitAct()
    {
        _timeSinceLastWater = 0;
        _viewManager.ToggleNavButtons(true);
        _plantObject.InitPlant();
        _computerObject.EnableComputer(false);
        _waterCoolerObject.EnableWaterCooler();
    }


    private IEnumerator FadeOutCoroutine()
    {
        _animator.SetTrigger("EndOfDay");
        yield return new WaitForSecondsRealtime(1.5f);

        GameEvents.ActOver();
    }

    private void EndTutorial()
    {
        _isInTutorial = false;
        _actNumber = 1;
        _storyBeat = 0;
        GameEvents.ProgressStory();
    }


    private void NextStoryBeat()
    {

        if (_isInTutorial)
        {
            GameEvents.NextTutorialStep();
        }
        else
        {
        
            _storyBeat++;
            _timeSinceLastWater++;

            if (_actsActionCount[_actNumber] == _storyBeat)
            {
                StartCoroutine(FadeOutCoroutine());
            }
            else
            {
                Debug.Log("I'm the next story beat! " + _storyBeat);
                SetupStoryBeat();
            }
        }

    }


    private void SetupStoryBeat()
    {
        _debugger.UpdateDebugText($"Act {_actNumber} Beat {_storyBeat}");

        // init
        
        _computerObject.CheckForNewEmails();
       

        // disable all 

        switch (_actNumber)
        {
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

        if (_timeSinceLastWater >= _minTimeBetweenWaterings + 6)
        {
            _plantObject.PlantDies();
        }
        else if (_timeSinceLastWater >= _minTimeBetweenWaterings + 2)
        {
            _plantObject.UpdatePlantStatus(true);
        }
        else if (_timeSinceLastWater == _minTimeBetweenWaterings)
        {
            _waterCoolerObject.EnableWaterCooler();
        }
        
    }


    private void SetupAct1StoryBeat()
    {
        Debug.Log($"setting up story beat {_storyBeat} in act {_actNumber}");

        switch (_storyBeat)
        {
            case 1:

                //_viewManager.ToggleNavButtons(true);
                //_computerObject.EnableComputer();
                //_viewManager.ToggleNavButtons(true);
                //SetupTimedEvent(2f, BobVisit1);

                //_phoneObject.ReceiveBossPhoneCall(10);

                //SetupTimedEvent(3f, DoMedsAlarm);
                

                break;

            case 2:

                TimedCoworkerInteraction(2f, CoWorker.SUSAN, 5);


                //_phoneObject.PhoneTutorial();
                //TutorialHandler.Instance.StartTutorial(2);
                //SetupTutorial(2, 1f);
                //_phoneObject.ReceivePhoneCall(1);

                break;

            case 3:

                // phone call?
                break;

            case 4:


                TimedCoworkerInteraction(2f, CoWorker.MARGE, 4);
                break;

            case 5:

                SetupTimedEvent(5f, DoMedsAlarm);
                break;

            case 6:

                _computerObject.EnableComputer(false);
                TimedComputerCrash(3f);
                break;

            case 7:

                _cupObject.EnableCup();
                // computer crash

                //_interactionID = 4;
                //SetupTimedEvent(4f, BobVisit);
                TimedBobVisit(4f, 4);

                //_bobInteraction.DoVisit(4);
                break;

            case 8:

                ReceiveTimedPhoneCall(2f, 1);
                break;

            case 9:

                TimedCoworkerInteraction(3f, CoWorker.MATT, 1);
                
                break;

            case 10:

                TimedBobVisit(3f, 5);
                break;

            case 11:

                _computerObject.FixCrash();
                break;

            case 12:

                _phoneObject.ReceiveBossPhoneCall(2);
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

                // jeremy 2 email?
                break;

            case 2:

                //StartCoroutine(GameManager.Instance.BobVisit1());
                break;

            case 3:

                //GameEvents.MedsAlarmGoesOff(1);
                _margeGossipInteraction.StartGossip(2);
                break;

            case 4:

                break;

            case 5:

                SetupTimedEvent(3f, DoMedsAlarm);
                break;

            case 6:

                ReceiveTimedPhoneCall(2f, 3);

                break;

            case 7:

                _bobInteraction.DoVisit(6);
                break;

            case 8:

                break;

            case 9:

                break;

            case 10:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    _phoneObject.ReceiveBossPhoneCall(6);
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    _phoneObject.ReceiveBossPhoneCall(7);
                }
                else
                {
                    _phoneObject.ReceiveBossPhoneCall(8);
                }
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


                break;

            case 2:

               // GameManager.Instance.BobVisit1();
                break;

            case 3:

                
                break;

            case 4:

                break;

            case 5:

                SetupTimedEvent(3f, DoMedsAlarm);
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

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    _phoneObject.ReceiveBossPhoneCall(9);
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    _phoneObject.ReceiveBossPhoneCall(10);
                }
                else
                {
                    _phoneObject.ReceiveBossPhoneCall(11);
                }
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


    
    private void WaterPlant()
    {
        PlayerChoices.Instance.ChangePlayerValue("WateredCount", 1);
        _timeSinceLastWater = 0;
    }


    public void TimedComputerCrash(float timer)
    {
        Debug.Log($"Set up timed computer crash: timer={timer}");
        _timeToEvent = timer;
        _timedEvent = ComputerCrashes;
        _timerOn = true;

    }

    private void ComputerCrashes()
    {
        _computerObject.DoCrash();
    }


    private void ReceiveTimedPhoneCall(float timer, int phoneInteractionID)
    {
        Debug.Log($"Set up timed phone call: timer={timer}");
        _timeToEvent = timer;
        _interactionID = phoneInteractionID;
        _timedEvent = ReceivePhoneCall;
        _timerOn = true;

    }

    private void TimedCoworkerInteraction(float timer, CoWorker coworker, int workerInteractionID)
    {
        Debug.Log($"Set up timed co worker: timer={timer}");
        _timeToEvent = timer;
        _interactionID = workerInteractionID;
        _activeCoWorkerID = coworker;
        _timedEvent = CoworkerInteraction;
        _timerOn = true;
    }


    private void TimedBobVisit(float timer, int bobInteractionID)
    {
        Debug.Log($"Set up timed bob visit: timer={timer}");
        _timeToEvent = timer;
        _interactionID = bobInteractionID;
        _timedEvent = BobVisit;
        _timerOn = true;
    }


    private void ReceivePhoneCall()
    {
        _phoneObject.ReceivePhoneCall(_interactionID);
    }

    private void CoworkerInteraction()
    {
        _coWorkers[(int)_activeCoWorkerID].StartGossip(_interactionID);
    }





    public void BobVisit()
    {
        _bobInteraction.DoVisit(_interactionID);
    }


    private void DoMedsAlarm()
    {

        _computerObject.DisableComputer();
        _cupObject.DisableCup();
        GameEvents.MedsAlarmGoesOff(_alarmID);
        _alarmID++;
    }

}

public enum PlayerState
{
    IDLE,
    BUSY
}

public enum CoWorker
{
    SIMON,
    MARGE,
    SUSAN,
    MATT
}
