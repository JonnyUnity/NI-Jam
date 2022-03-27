using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class DialogueHandler : Singleton<DialogueHandler>
{
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private GameObject _nameBox;
    [SerializeField] private TMP_Text _textLabel;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private GameObject _choicesBox;
    [SerializeField] private GameObject _choiceButtonsContainer;
    [SerializeField] private GameObject _choiceButtonTemplate;
    private List<GameObject> _choiceButtons = new List<GameObject>();

    [SerializeField] private GameObject _responseBox;
    [SerializeField] private TMP_Text _responseText;

    private AudioSource _dialogueAudio;
    private AudioClip _otherVoiceClip;
    [SerializeField] private AudioClip _playerVoice;

    [SerializeField] private AudioClip _bobVoice;
    //[SerializeField] private AudioClip _neutralVoice;
    [SerializeField] private AudioClip _bossVoice;

    private TypeWriterEffect _dialogueWriter;
    private TypeWriterEffect _responseWriter;

    private bool _isResponse;

    private int ptr;

    private List<Dialogue> _dialogues;
    private Dialogue _currentDialogue;
    private Response _currentResponse;

    private int _interactionObjectID;

    public bool IsDialogueOpen => _dialogueBox.activeInHierarchy || _choicesBox.activeInHierarchy || _responseBox.activeInHierarchy;

    private void Awake()
    {
        GameEvents.OnDialogueStarted += DialogueStarted;
        //GameEvents.OnDialogueEnded += DialogueEnded;
        
        ptr = 0;
        _dialogueWriter = _dialogueBox.GetComponent<TypeWriterEffect>();
        _responseWriter = _responseBox.GetComponent<TypeWriterEffect>();
        _dialogueAudio = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_canvas);
        DontDestroyOnLoad(_dialogueBox);
        DontDestroyOnLoad(_choicesBox);
        DontDestroyOnLoad(_responseBox);
        DontDestroyOnLoad(_dialogueAudio);
    }


    private void OnDisable()
    {
        GameEvents.OnDialogueStarted -= DialogueStarted;

    }


    private void DialogueStarted()
    {
         _dialogueBox.SetActive(true);
    }


    private void DialogueEnded()
    {
        _dialogueBox.SetActive(false);

        ClearChoices();

        _choicesBox.SetActive(false);
        _responseBox.SetActive(false);

        GameEvents.DialogueEnded(_interactionObjectID);
    }

    private void ClearChoices()
    {
        foreach (GameObject btn in _choiceButtons)
        {
            Destroy(btn);
        }
        _choiceButtons.Clear();
    }


    
    public void StartDialogue(List<Dialogue> dialogues, AudioClip voiceClip, int interactionObjectID)
    {
        _interactionObjectID = interactionObjectID;
        _dialogues = dialogues;
        _currentDialogue = _dialogues[0];
        _otherVoiceClip = voiceClip;

        SetupDialogue();

    }


    public void ContinueDialogue(int dialogueID)
    {
        _currentDialogue = _dialogues.Where(w => w.ID == dialogueID).Single();
        SetupDialogue();
    }    


    private void SetupDialogue()
    {
        GameEvents.DialogueStarted();
        ptr = 0;

        _choicesBox.SetActive(false);
        _responseBox.SetActive(false);

        AdvanceDialogue();

    }


    void Update()
    {
        //if (TutorialHandler.Instance.IsTutorialOpen)
        //    return;

        if (IsDialogueOpen)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_dialogueWriter.IsTyping)
                {

                    _dialogueWriter.FinishTyping();

                }
                else if (_isResponse)
                {
                    if (_responseWriter.IsTyping)
                    {
                        _responseWriter.FinishTyping();
                    }
                    else if (IsMoreResponse)
                    {
                        AdvanceResponse();
                    }
                    else
                    {
                        _isResponse = false;
                        _currentResponse.Actions.ForEach(f => f.DoAction());
                        _currentResponse.OnSelect?.Invoke();
                        _responseBox.SetActive(false);

                        //GameEvents.DialogueEnded(); // is this going to cause a problem?
                    }
                }
                else if (IsMoreDialogue)
                {

                    AdvanceDialogue();

                }
                else if (_currentDialogue.HasResponses)
                {
                    _dialogueBox.SetActive(false);

                    var responses = _currentDialogue.Responses;
                    if (responses.Count == 1)
                    {
                        // exactly one response so skip choice and play dialogue response
                        SetupResponse(responses[0].ID);

                    }
                    else
                    {
                        // multiple to choose from so offer choices to player
                        // set up choice box...
                        
                        _choicesBox.SetActive(true);
                        float buttonContainerHeight = 0;

                        for (int i = 0; i < responses.Count; i++)
                        {
                            if (responses[i].IncludeResponse())
                            {

                                GameObject choiceButton = Instantiate(_choiceButtonTemplate, _choiceButtonsContainer.transform);

                                int responseID = responses[i].ID;
                                var btn = choiceButton.GetComponent<Button>();
                                btn.GetComponentInChildren<TMP_Text>().text = responses[i].Text;
                                btn.onClick.AddListener(() => SetupResponse(responseID));

                                _choiceButtons.Add(choiceButton);
                                buttonContainerHeight += choiceButton.GetComponent<RectTransform>().sizeDelta.y;

                            }

                        }

                        Vector2 choicesBoxDimensions = _choicesBox.GetComponent<RectTransform>().sizeDelta;
                        _choicesBox.GetComponent<RectTransform>().sizeDelta = new Vector2(choicesBoxDimensions.x, buttonContainerHeight + 40);
                    }

                }
                else
                {
                    DialogueEnded();

                    if (_currentDialogue.EndOfConversation)
                    {
                        GameEvents.DialogueEnded(_interactionObjectID);
                    }
                    //GameEvents.DialogueEnded();

                    if (_currentDialogue.AdvanceStoryOnClose)
                    {
                        //GameEvents.ProgressStory();
                        StartCoroutine(PauseBeforeProgressingStory());
                    }
                }

            }
        }

        
    }

    private IEnumerator PauseBeforeProgressingStory()
    {
        yield return new WaitForSeconds(0.25f);

        GameEvents.ProgressStory();
    }


    private void AdvanceDialogue()
    {
        _dialogueBox.SetActive(true);
        if (_currentDialogue.Speaker == null || _currentDialogue.Speaker == string.Empty)
        {
            _nameBox.SetActive(false);
            _dialogueAudio.clip = _playerVoice;
        }
        else
        {
            _nameBox.SetActive(true);
            _nameLabel.text = _currentDialogue.Speaker;
            _dialogueAudio.clip = _otherVoiceClip;
        }
                
        _dialogueWriter.Run(_currentDialogue.Sentences[ptr], _textLabel, _dialogueAudio);
        ptr++;

    }


    private void AdvanceResponse()
    {
        _responseWriter.Run(_currentResponse.AnswerSentences[ptr], _responseText, _dialogueAudio);
        ptr++;
    }


    private bool IsMoreDialogue => ptr < _currentDialogue.Sentences.Length;
    

    private bool IsMoreResponse => ptr < _currentResponse.AnswerSentences.Length;


    public void SetupResponse(int responseID)
    {

         _currentResponse = _currentDialogue.Responses.Where(w => w.ID == responseID).Single();

        _choicesBox.SetActive(false);
        _responseBox.SetActive(true);

        ClearChoices();

        ptr = 0;
        _isResponse = true;
        _dialogueAudio.clip = _playerVoice;
        AdvanceResponse();

    }


    public void PlayResponseSoundEffect(AudioClip clip)
    {
        _dialogueAudio.PlayOneShot(clip);
    }


}
