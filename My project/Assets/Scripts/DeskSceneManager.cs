using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskSceneManager : Singleton<DeskSceneManager>
{

    //private static DeskSceneManager _Instance;
    //public static DeskSceneManager Instance
    //{
    //    get
    //    {
    //        if (!_Instance)
    //        {
    //            _Instance = new GameObject().AddComponent<DeskSceneManager>();
    //            _Instance.name = _Instance.GetType().ToString();

    //            DontDestroyOnLoad(_Instance.gameObject);
    //        }

    //        return _Instance;
    //    }
    //}


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
    [SerializeField] private Medicine _medsAlarmObject;

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
    private bool _isLizardPerson;

    private int _alarmID = 1;

    private bool _isInTutorial;

    // Plant
    [SerializeField] private int _minTimeBetweenWaterings;
    private int _timeSinceLastWater;

    private static readonly string[] _actTitles = { "Monday", "Wednesday", "Friday" };

    private static readonly int[] _actsActionCount = new int[]
    {
        0, 13, 12, 8
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
        _plantObject.InitPlant();
        _computerObject.InitEmails();
        yield return new WaitForSecondsRealtime(2f);

        _audioSource.Play();
        
        if (actNumber == 1)
        {

            if (_skipTutorial)
            {
                InitAct();

                _isInTutorial = false;
                _actNumber = 1;
                _storyBeat = 1;

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
        //_plantObject.InitPlant();
        _computerObject.EnableComputer(false);
        _waterCoolerObject.EnableWaterCooler();
    }


    private IEnumerator FadeOutCoroutine()
    {
        _animator.SetTrigger("EndOfDay");
        var t = 0;

        var origVolume = _audioSource.volume;
        for (var timePassed = 0f; timePassed < 1.5f; timePassed += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(origVolume, 0f, timePassed / 1.5f);

            yield return null;
        }

        //yield return new WaitForSecondsRealtime(1.5f);
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

                // email
                break;

            case 2:

                TimedCoworkerInteraction(2f, CoWorker.SUSAN, 5, false);
                break;

            case 3:

                // email
                break;

            case 4:

                TimedCoworkerInteraction(2f, CoWorker.MARGE, 4, false);
                break;

            case 5:

                TimedMedsAlarm(5f, 1);
                break;

            case 6:

                _computerObject.EnableComputer(false);
                TimedComputerCrash(3f);
                break;

            case 7:

                _cupObject.EnableCup();
                TimedBobVisit(4f, 4, false);
                break;

            case 8:

                ReceiveTimedPhoneCall(4f, 1);
                break;

            case 9:

                TimedCoworkerInteraction(3f, CoWorker.MATT, 1, false);   
                break;

            case 10:

                TimedBobVisit(3f, 5, false);
                break;

            case 11:

                _computerObject.FixCrash();
                // email
                break;

            case 12:

                ReceiveTimedBossPhoneCall(2f, 2);
                //_phoneObject.ReceiveBossPhoneCall(2);
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

                TimedBobVisit(2f, 17, false);
                break;

            case 2:

                // Matt
                TimedCoworkerInteraction(2f, CoWorker.MATT, 18, false);              
                break;

            case 3:

                TimedCoworkerInteraction(2f, CoWorker.SIMON, 15, false);
                break;

            case 4:

                ReceiveTimedPhoneCall(2f, 18);
                break;

            case 5:

                // decision point = lizardman story
                TimedMedsAlarm(3f, 2);

                break;

            case 6:

                _computerObject.EnableComputer(false);
                PlayerChoices.Instance.CheckLizardmanStoryLine();

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    GameEvents.ProgressStory();
                }
                else
                {
                    // decision point = hitman story
                    TimedBobVisit(3f, 6, false);

                }
                break;

            case 7:

                PlayerChoices.Instance.CheckHitmanStoryLine();

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    ReceiveTimedPhoneCall(2f, 12);
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    //ReceiveTimedPhoneCall(2f, 13);
                    GameEvents.ProgressStory();
                }
                else
                {
                    ReceiveTimedPhoneCall(2f, 14);
                }

                break;

            case 8:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    //coworker (temperature)
                    TimedCoworkerInteraction(2f, CoWorker.SUSAN, 16, false);
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {

                    // jeremy virus call
                    ReceiveTimedPhoneCall(2f, 19);
                    _computerObject.EnableVirus();
                }
                else
                {
                    // bob, you're strange
                    GameEvents.ProgressStory();
                }

                break;

            case 9:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    // bob "you don't look so well!"
                    TimedBobVisit(2f, 7, false);
                }
                //else if (PlayerChoices.Instance.OnHitmanStory)
                //{
                //    Debug.Log("Use virus!");
                // progressed via virus

                //}
                else if (PlayerChoices.Instance.OnClonesStory)
                {
                    // coworker chat - bob strange
                    TimedCoworkerInteraction(2f, CoWorker.MARGE, 6, false);
                }
                //else
                //{
                //    GameEvents.ProgressStory();
                //}
                break;

            case 10:
      
                if (PlayerChoices.Instance.OnHitmanStory)
                {
                    // bob talks about stuff he's found!
                    TimedBobVisit(2f, 8, false);
                }
                else // clones
                {
                    GameEvents.ProgressStory();
                }
                break;

            case 11:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    //_phoneObject.ReceiveBossPhoneCall(6);
                    ReceiveTimedBossPhoneCall(2f, 6);
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    //_phoneObject.ReceiveBossPhoneCall(7);
                    ReceiveTimedBossPhoneCall(2f, 7);
                }
                else
                {
                    //_phoneObject.ReceiveBossPhoneCall(8);
                    ReceiveTimedBossPhoneCall(2f, 8);
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

        if (PlayerChoices.Instance.OnLizardmanStory)
        {
            // phone call
            switch (_storyBeat)
            {
                case 1:

                    // bob visit
                    TimedBobVisit(2f, 9, false);
                    break;

                case 2:

                    // phone call
                    ReceiveTimedPhoneCall(2f, 15);
                    break;

                //case 3:

                //    // email
                //    Debug.Log("email!");
                //    GameEvents.ProgressStory();
                //    break;

                case 3:

                    // coworker
                    TimedCoworkerInteraction(2f, CoWorker.MARGE, 7, false);
                    break;

                case 4:

                    TimedMedsAlarm(3f, 4);
                    break;

                case 5:

                    _computerObject.EnableComputer(false);
                    // coworker
                    TimedCoworkerInteraction(2f, CoWorker.MATT, 8, true);
                    break;

                case 6:

                    // green bob
                    TimedBobVisit(2f, 12, true);
                    break;

                //case 8:

                //    // weird stuff on walls? skip?
                //    Debug.Log("Act 3 beat 8 lizardman - weird stuff on walls?");
                //    GameEvents.ProgressStory();
                //    break;

                //case 9:

                //    // green co-worker
                //    TimedCoworkerInteraction(2f, CoWorker.SIMON, 9, true);
                //    break;

                case 7:

                    ReceiveTimedBossPhoneCall(2f, 9);
                    //_phoneObject.ReceiveBossPhoneCall(9);
                    break;

                default:
                    Debug.Log("Out of story beats! Shouldn't be here!");
                    break;

            }

        }
        else if (PlayerChoices.Instance.OnHitmanStory)
        {
            switch (_storyBeat)
            {
                case 1:

                    // bob visit
                    //TimedBobVisit(2f, 10, false);

                    // bob talks about someone after him!
                    TimedBobVisit(2f, 13, false);
                    break;



                case 2:

                    // jeremy calls
                    ReceiveTimedPhoneCall(2f, 16);
                    break;

                //case 3:

                    
                //    break;

                case 3:

                    // marge someone odd at reception
                    TimedCoworkerInteraction(2f, CoWorker.MARGE, 10, false);
                    break;

                case 4:

                    TimedMedsAlarm(3f, 3);
                    break;

                //case 6:

                //    // bob swap plant option?
                //    TimedBobVisit(2f, 14, false);
                //    break;

                //case 7:

                //    // skip?
                //    GameEvents.ProgressStory();
                //    break;

                case 5:

                    _computerObject.EnableComputer(false);
                    // matt coworker
                    TimedCoworkerInteraction(2f, CoWorker.MATT, 11, false);
                    break;

                case 6:

                    // bob visit - don't do anything, I need to go chat to the boss
                    TimedBobVisit(2f, 15, false);
                    break;

                case 7:

                    ReceiveTimedBossPhoneCall(4f, 10);
                    //_phoneObject.ReceiveBossPhoneCall(10);
                    break;

                default:
                    Debug.Log("Out of story beats! Shouldn't be here!");
                    break;

            }

        }
        else
        {
            switch (_storyBeat)
            {
                case 1:

                    // bob visit
                    TimedBobVisit(2f, 11, false);
                    break;

                case 2:

                    // bob call
                    ReceiveTimedPhoneCall(2f, 17);
                    break;

                //case 3:

                //    // email?
                //    Debug.Log("Act 3 Beat 3 Email?");
                //    GameEvents.ProgressStory();
                //    break;

                case 3:

                    // co-worker marge
                    TimedCoworkerInteraction(2f, CoWorker.MARGE, 12, false);
                    break;

                case 4:

                    TimedMedsAlarm(3f, 3);
                    break;

                //case 6:

                //    // email
                //    Debug.Log("Act 3 Beat 6 Email?");
                //    GameEvents.ProgressStory();
                //    break;

                case 5:

                    _computerObject.EnableComputer(false);
                    // bob visit
                    TimedBobVisit(2f, 16, false);
                    break;

                //case 8:

                //    // ???
                //    Debug.Log("Clones Act 3 beat 8 - nothing here!");
                //    GameEvents.ProgressStory();
                //    break;

                case 6:

                    // matt as bob visit
                    TimedCoworkerInteraction(2f, CoWorker.BOBLEFT, 13, false);
                    break;

                case 7:

                    ReceiveTimedBossPhoneCall(2f, 11);
                    //_phoneObject.ReceiveBossPhoneCall(11);
                    break;

                default:
                    Debug.Log("Out of story beats! Shouldn't be here!");
                    break;

            }
        }

    }

    //private void SetupAct3StoryBeat()
    //{
    //    Debug.Log($"setting up story beat {_storyBeat} in act {_actNumber}");

    //    if (PlayerChoices.Instance.OnLizardmanStory)
    //    {
    //        // phone call
    //        switch (_storyBeat)
    //        {
    //            case 1:

    //                // bob visit
    //                TimedBobVisit(2f, 9, false);
    //                break;

    //            case 2:

    //                // phone call
    //                ReceiveTimedPhoneCall(2f, 15);
    //                break;

    //            case 3:

    //                // email
    //                Debug.Log("email!");
    //                GameEvents.ProgressStory();
    //                break;

    //            case 4:

    //                // coworker
    //                TimedCoworkerInteraction(2f, CoWorker.MARGE, 7, false);
    //                break;

    //            case 5:

    //                TimedMedsAlarm(3f, 4);
    //                break;

    //            case 6:

    //                // coworker
    //                TimedCoworkerInteraction(2f, CoWorker.MATT, 8, false);
    //                break;

    //            case 7:

    //                // green bob
    //                TimedBobVisit(2f, 12, true);
    //                break;

    //            case 8:

    //                // weird stuff on walls? skip?
    //                Debug.Log("Act 3 beat 8 lizardman - weird stuff on walls?");
    //                GameEvents.ProgressStory();
    //                break;

    //            case 9:

    //                // green co-worker
    //                TimedCoworkerInteraction(2f, CoWorker.SIMON, 9, true);
    //                break;

    //            case 10:

    //                _phoneObject.ReceiveBossPhoneCall(9);
    //                break;

    //            default:
    //                Debug.Log("Out of story beats! Shouldn't be here!");
    //                break;

    //        }

    //    }
    //    else if (PlayerChoices.Instance.OnHitmanStory)
    //    {
    //        switch (_storyBeat)
    //        {
    //            case 1:

    //                // bob visit
    //                TimedBobVisit(2f, 10, false);
    //                break;

    //            case 2:

    //                // jeremy calls
    //                ReceiveTimedPhoneCall(2f, 16);
    //                break;

    //            case 3:

    //                // bob talks about someone after him!
    //                TimedBobVisit(2f, 13, false);
    //                break;

    //            case 4:

    //                // marge someone odd at reception
    //                TimedCoworkerInteraction(2f, CoWorker.MARGE, 10, false);
    //                break;

    //            case 5:

    //                TimedMedsAlarm(3f, 3);
    //                break;

    //            case 6:

    //                // bob swap plant option?
    //                TimedBobVisit(2f, 14, false);
    //                break;

    //            case 7:

    //                // skip?
    //                GameEvents.ProgressStory();
    //                break;

    //            case 8:

    //                // matt coworker
    //                TimedCoworkerInteraction(2f, CoWorker.MATT, 11, false);
    //                break;

    //            case 9:

    //                // bob visit - don't do anything, I need to go chat to the boss
    //                TimedBobVisit(2f, 15, false);
    //                break;

    //            case 10:

    //                _phoneObject.ReceiveBossPhoneCall(10);
    //                break;

    //            default:
    //                Debug.Log("Out of story beats! Shouldn't be here!");
    //                break;

    //        }

    //    }
    //    else
    //    {
    //        switch (_storyBeat)
    //        {
    //            case 1:

    //                // bob visit
    //                TimedBobVisit(2f, 11, false);
    //                break;

    //            case 2:

    //                // bob call
    //                ReceiveTimedPhoneCall(2f, 17);
    //                break;

    //            case 3:

    //                // email?
    //                Debug.Log("Act 3 Beat 3 Email?");
    //                GameEvents.ProgressStory();
    //                break;

    //            case 4:

    //                // co-worker marge
    //                TimedCoworkerInteraction(2f, CoWorker.MARGE, 12, false);
    //                break;

    //            case 5:

    //                TimedMedsAlarm(3f, 3);
    //                break;

    //            case 6:

    //                // email
    //                Debug.Log("Act 3 Beat 6 Email?");
    //                GameEvents.ProgressStory();
    //                break;

    //            case 7:

    //                // bob visit
    //                TimedBobVisit(2f, 16, false);
    //                break;

    //            case 8:

    //                // ???
    //                Debug.Log("Clones Act 3 beat 8 - nothing here!");
    //                GameEvents.ProgressStory();
    //                break;

    //            case 9:

    //                // matt as bob visit
    //                TimedCoworkerInteraction(2f, CoWorker.MATT, 13, false);
    //                break;

    //            case 10:

    //                _phoneObject.ReceiveBossPhoneCall(11);
    //                break;

    //            default:
    //                Debug.Log("Out of story beats! Shouldn't be here!");
    //                break;

    //        }
    //    }

    //}


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

    private void ReceiveTimedBossPhoneCall(float timer, int phoneInteractionID)
    {
        Debug.Log($"Set up timed phone call: timer={timer}");
        _timeToEvent = timer;
        _interactionID = phoneInteractionID;
        _timedEvent = ReceiveBossPhoneCall;
        _timerOn = true;

    }


    private void TimedCoworkerInteraction(float timer, CoWorker coworker, int workerInteractionID, bool isLizard)
    {
        Debug.Log($"Set up timed co worker: timer={timer}");
        _timeToEvent = timer;
        _interactionID = workerInteractionID;
        _activeCoWorkerID = coworker;
        _timedEvent = CoworkerInteraction;
        _isLizardPerson = isLizard;
        _timerOn = true;
    }


    private void TimedBobVisit(float timer, int bobInteractionID, bool isLizard)
    {
        Debug.Log($"Set up timed bob visit: timer={timer}");
        _timeToEvent = timer;
        _interactionID = bobInteractionID;
        _timedEvent = BobVisit;
        _isLizardPerson = isLizard;
        _timerOn = true;
    }


    private void ReceivePhoneCall()
    {
        _phoneObject.ReceivePhoneCall(_interactionID);
    }



    private void ReceiveBossPhoneCall()
    {
        _phoneObject.ReceiveBossPhoneCall(_interactionID);
    }

    private void CoworkerInteraction()
    {
        _coWorkers[(int)_activeCoWorkerID].StartGossip(_interactionID, _isLizardPerson);
    }


    public void BobVisit()
    {
        _bobInteraction.DoVisit(_interactionID, _isLizardPerson);
    }


    private void DoMedsAlarm()
    {

        _computerObject.DisableComputer();
        _cupObject.DisableCup();
        GameEvents.MedsAlarmGoesOff(_alarmID);
        //_medsAlarmObject.StartAlarm(_alarmID);
    }

    private void TimedMedsAlarm(float timer, int alarmID)
    {
        Debug.Log($"Set up timed meds alarm: timer={timer}");
        _timeToEvent = timer;
        _alarmID = alarmID;
        _timedEvent = DoMedsAlarm;
        _timerOn = true;
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
    MATT,
    BOBLEFT
}
