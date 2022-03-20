using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TutorialHandler : Singleton<TutorialHandler>
{

    [SerializeField] private GameObject _tutorialBox;
    [SerializeField] private TMP_Text _tutorialText;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartTutorial(int tutorialID)
    {
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
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
                if (_currentDialogue.AdvanceStoryOnClose)
                {
                    GameEvents.ProgressStory();
                }
                _tutorialText.text = string.Empty;
                _tutorialBox.SetActive(false);
            }
        }

    }

    private void AdvanceDialogue()
    {
        _tutorialBox.SetActive(true);

        _writer.Run(_currentDialogue.Sentences[ptr], _tutorialText);
        ptr++;

    }

}
