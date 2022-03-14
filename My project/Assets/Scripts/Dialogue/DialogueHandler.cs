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

    [SerializeField] private GameObject _responseBox;
    [SerializeField] private List<GameObject> _responseButtons;
    [SerializeField] private List<TMP_Text> _responseButtonLabels; 

    private TypeWriterEffect _typeWriter;

    private int ptr;

    private List<Dialogue> _dialogues;
    private Dialogue _currentDialogue;
    private List<Response> _responses;

    private void Awake()
    {
        GameEvents.OnDialogueStarted += DialogueStarted;
        GameEvents.OnDialogueEnded += DialogueEnded;
        
        ptr = 0;
        _typeWriter = _dialogueBox.GetComponent<TypeWriterEffect>();

        DontDestroyOnLoad(gameObject);



    }



    private void DialogueStarted()
    {
        _dialogueBox.SetActive(true);
    }

    private void DialogueEnded()
    {
        _dialogueBox.SetActive(false);
        _responseBox.SetActive(false);
    }

    void Start()
    {
        
    }

    public void StartDialogue(List<Dialogue> dialogues, List<Response> responses)
    {
        _dialogues = dialogues;
        _responses = responses;
        ptr = 0;

        _responseBox.SetActive(false);
        _dialogueBox.SetActive(true);

        // load first dialogue.
        _currentDialogue = _dialogues[0];

        SetupCurrentDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_typeWriter.IsTyping)
            {

                _typeWriter.FinishTyping();

            }
            else if (IsMoreDialogue())
            {

                SetupCurrentDialogue();

            }
            else if (DialogueHasLinks())
            {
                var link = _currentDialogue.Links;

                switch (link.Type)
                {
                    case DialogueNodeType.Dialogue:

                        var nextID = link.IDs.First();
                        _currentDialogue = _dialogues.Where(w => w.ID == nextID).Single();
                        ptr = 0;
                        SetupCurrentDialogue();
                        break;

                    case DialogueNodeType.Response:

                        var responses = _responses.Where(w => link.IDs.Contains(w.ID)).ToList();

                        // set up response box...
                        _dialogueBox.SetActive(false);
                        _responseBox.SetActive(true);
                        _responseButtons.ForEach(f => f.SetActive(false));

                        for (int i = 0; i < responses.Count; i++)
                        {
                            if (responses[i].IncludeResponse())
                            {

                                _responseButtonLabels[i].text = responses[i].Answer;
                                _responseButtons[i].SetActive(true);
                                var btn = _responseButtons[i].GetComponent<Button>();
                                int responseID = responses[i].ID;
                                btn.onClick.AddListener(delegate { ProcessResponse(responseID); });
                            }

                        }
                        break;

                    case DialogueNodeType.None:
                        
                        GameEvents.DialogueEnded();
                        break;

                    default:

                        Debug.Log($"Link type {link.Type} currently unsupported!");
                        break;
                }
            }
            else
            {
                GameEvents.DialogueEnded();
            }


        }
    }

    private void SetupCurrentDialogue()
    {
        if (_currentDialogue.Speaker == null || _currentDialogue.Speaker == string.Empty)
        {
            _nameBox.SetActive(false);
        }
        else
        {
            _nameBox.SetActive(true);
            _nameLabel.text = _currentDialogue.Speaker;
        }

        _typeWriter.Run(_currentDialogue.Sentences[ptr], _textLabel);
        ptr++;

    }

    private bool DialogueHasLinks()
    {
        return (_currentDialogue.Links != null);
    }


    private bool IsMoreDialogue()
    {
        //return ptr < _debugdialogue.Length;
        //return ptr < _dialogueOld.Text.Length;
        return ptr < _currentDialogue.Sentences.Length;
    }


    public void ProcessResponse(int responseID)
    {
        Response response = _responses.Where(w => w.ID == responseID).Single();

        var action = response.Action;
        _responseBox.SetActive(false);

         if (action.Type == ResponseActionTypeEnum.SetFlag)
        {
            //GameEvents.SetPlayerFlag(action.Choice);
            PlayerChoices.Instance.SetPlayerFlag(action.Choice);
        }
        else if (action.Type == ResponseActionTypeEnum.UpdateCount)
        {
            //GameEvents.ChangePlayerValue(action.Choice, action.ChangeInValue);
            PlayerChoices.Instance.ChangePlayerValue(action.Choice, action.ChangeInValue);
        }

        if (action.DialogueID > 0)
        {
            _currentDialogue = _dialogues.Where(w => w.ID == action.DialogueID).Single();
            ptr = 0;
            SetupCurrentDialogue();
        }
        else
        {

            response.OnSelect?.Invoke(responseID);
            GameEvents.DialogueEnded();
        }

        


    }

}
