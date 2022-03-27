using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class TutorialHandler : MonoBehaviour
{

    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private Phone _phoneObject;
    [SerializeField] private Computer _computerObject;
    [SerializeField] private Plant _plantObject;
    [SerializeField] private Watercooler _watercoolerObject;
    [SerializeField] private FullCup _fullCupObject;

    [SerializeField] private CoWorkerInteraction _bobInteraction;
    [SerializeField] private CoWorkerInteraction _gossipInteraction;


    [SerializeField] private GameObject _tutorialBox;
    [SerializeField] private TMP_Text _tutorialText;

    private int _tutorialStep;
    private int _interactionID;

    private TypeWriterEffect _writer;
    private int ptr;

    [SerializeField] private List<Dialogue> _dialogues;
    private Dialogue _currentDialogue;

    public bool IsTutorialOpen => _tutorialBox.activeInHierarchy;

    private bool IsMoreDialogue => ptr < _currentDialogue.Sentences.Length;

    private void Awake()
    {
        _writer = _tutorialBox.GetComponent<TypeWriterEffect>();
    }

    private void OnEnable()
    {
        GameEvents.OnTutorialStarted += StartTutorial;
        GameEvents.OnNextTutorialStep += NextTutorialStep;

    }


    private void OnDisable()
    {
        GameEvents.OnTutorialStarted -= StartTutorial;
        GameEvents.OnNextTutorialStep -= NextTutorialStep;
    }


    public void StartTutorial()
    {
        // other init?
        _watercoolerObject.EnableTutorial();
        _viewManager.EnableTutorial();
        _fullCupObject.UseTutorialDialogue();
        
        NextTutorialStep();

    }


    private void NextTutorialStep()
    {
        _tutorialStep++;
        Debug.Log($"setting up tutorial step {_tutorialStep}");

         switch (_tutorialStep)
        {
            case 1:

                StartTutorialDialogue(1);
                break;

            case 2:

                _phoneObject.PhoneTutorial();
                StartTutorialDialogue(2);
                break;

            case 3:

                _phoneObject.ReceivePhoneCall(99);
                break;

            case 4:

                StartCoroutine(StartNextTutorialAfterDelay(3, 1));
                break;

            case 5:

                _viewManager.EnableLeftArrow();
                break;

            case 6:

                StartTutorialDialogue(4);
                break;

            case 7:

                _watercoolerObject.EnableWaterCooler();
                break;

            case 8:

                StartTutorialDialogue(5);
                break;

            case 9:

                _viewManager.EnableRightArrow();
                //GameEvents.TutorialFinished();

                break;
            case 10:

                StartCoroutine(StartNextTutorialAfterDelay(6, 1));
                break;

            case 11:

                StartCoroutine(StartNextTutorialAfterDelay(7, 1));
                _viewManager.EnableLeftArrow();
                _gossipInteraction.ShowConversationAlert();
                break;

            case 12:

                //_viewManager.EnableLeftArrow();
                _gossipInteraction.DoTutorial(1);
                break;

            case 13:

                StartCoroutine(StartNextTutorialAfterDelay(8, 1));
                break;

            case 14:

                StartCoroutine(StartNextTutorialAfterDelay(9, 1));
                break;

            case 15:

                _viewManager.EnableRightArrow();
                break;

            case 16:

                StartCoroutine(StartNextTutorialAfterDelay(10, 1));
                break;

            case 17:

                _computerObject.EnableComputer(true);
                break;

            case 18:

                StartTutorialDialogue(11);
                break;

            case 19:

                //StartTutorialDialogue(12);
                _computerObject.EnableDesktopIcons();
                break;

            case 20:

                Debug.Log("Open inbox!");
                StartTutorialDialogue(12);
                break;

            case 21:

                Debug.Log("Enable emails!");
                _computerObject.EnableEmailsForTutorial();
                break;

            case 22:

                Debug.Log("Open email!");
                StartTutorialDialogue(13);
                break;

            case 23:

                Debug.Log("Enable reply!");
                _computerObject.EnableReplyButtonForTutorial();
                break;

            case 24:

                StartTutorialDialogue(14);
                break;

            case 25:

                Debug.Log("Enable send!");
                _computerObject.EnableSendButtonForTutorial();
                break;

            //case 26:

            //    _computerObject.EnableCloseButtonsForTutorial();
            //    StartTutorialDialogue(15);
            //    break;

            case 26:

                _computerObject.DisableEmailsForTutorial();
                StartTutorialDialogue(16);
                break;

            case 27:

                _computerObject.EnableCloseButtonsForTutorial();
                _computerObject.EnableExitDesktop();
                break;

            case 28:

                StartTutorialDialogue(17);
                break;

            case 29:

                StartCoroutine(BobVisitCoroutine());
                break;

            case 30:

                GameEvents.TutorialFinished();
                break;

            default:
                Debug.Log($"No tutorial for step {_tutorialStep}");
                break;

        }

    }

    private IEnumerator StartNextTutorialAfterDelay(int tutorialID, float seconds)
    {

        yield return new WaitForSeconds(seconds);

        StartTutorialDialogue(tutorialID);
    
    }


    private IEnumerator BobVisitCoroutine()
    {
        yield return new WaitForSeconds(2f);

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


    public void StartTutorialDialogue(int tutorialID)
    {
        _tutorialText.text = string.Empty;
        _tutorialBox.SetActive(false);

        _currentDialogue = _dialogues.Where(w => w.ID == tutorialID).Single();

        ptr = 0;
        _tutorialBox.SetActive(true);
        AdvanceDialogue();

    }




    // Update is called once per frame
    void Update()
    {

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;

        if (IsTutorialOpen)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            //if (Input.GetMouseButtonDown(0))
            {

                Debug.Log("Pressed space!");

                if (_writer.IsTyping)
                {
                    _writer.FinishTyping();
                }
                else if (IsMoreDialogue)
                {
                    AdvanceDialogue();
                }
                else
                {

                    _tutorialText.text = string.Empty;
                    if (_currentDialogue.AdvanceStoryOnClose)
                    {


                        //GameEvents.NextTutorialStep();
                        StartCoroutine(PauseBetweenTutorialMessages());
                    }

                    _tutorialBox.SetActive(false);
                }
            }
        }
            

    }

    private void AdvanceDialogue()
    {
        _tutorialBox.SetActive(true);

        _writer.Run(_currentDialogue.Sentences[ptr], _tutorialText);
        ptr++;

    }


    private IEnumerator PauseBetweenTutorialMessages()
    {
        yield return new WaitForSeconds(1f);

        GameEvents.NextTutorialStep();
    }

}
