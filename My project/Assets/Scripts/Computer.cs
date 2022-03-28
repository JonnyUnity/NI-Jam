using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Computer : MonoBehaviour
{
    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private GameObject _desktopObject;
    [SerializeField] private GameObject _exitDesktopButton;

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _desktopOn;
    [SerializeField] private GameObject _desktopCrash;
    [SerializeField] private GameObject _emailNotificationObject;
    [SerializeField] private GameObject _virusScreenObject;

    [SerializeField] private Button _desktopIconEmail;
    [SerializeField] private Button _desktopIconGames;
    [SerializeField] private GameObject _desktopIconVirus;

    [SerializeField] private GameObject _inboxObject;
    [SerializeField] private GameObject _emailObject;
    [SerializeField] private TMP_Text _sender;
    [SerializeField] private TMP_Text _subject;
    [SerializeField] private TMP_Text _emailBody;
    [SerializeField] private GameObject _replyButton;
    [SerializeField] private Button[] _closeButtons;

    [SerializeField] private GameObject _replyObject;
    [SerializeField] private TMP_Text _replySender;
    [SerializeField] private TMP_Text _replySubject;
    [SerializeField] private TMP_Text _replyBody;
    [SerializeField] private GameObject _sendButton;

    [SerializeField] private GameObject _emailContainer;
    [SerializeField] private GameObject _emailTemplate;

    [SerializeField] private List<Email> _emails;

    private List<GameObject> _emailButtons = new List<GameObject>();
    private List<Email> _playerEmails;

    private Email _currentEmail;
    private GameObject _currentEmailButton;

    private BoxCollider2D _collider;
    [SerializeField] private Animator _animator;

    private bool _isTutorial;
    private bool _hasCrashed;

    private int _tutorialStep;
    private bool _advanceStoryOnClose;

    private bool StopInteracting => (EventSystem.current.IsPointerOverGameObject() || DialogueHandler.Instance.IsDialogueOpen || _hasCrashed);


    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    //private void OnEnable()
    //{
    //    // GameEvents.OnCloseDesktop += CloseDesktop;
    //    GameEvents.OnNextTutorialStep += NextTutorialStep;
    //}

    //private void OnDisable()
    //{
    //    // GameEvents.OnCloseDesktop -= CloseDesktop;
    //    GameEvents.OnNextTutorialStep -= NextTutorialStep;
    //}

    //private void NextTutorialStep()
    //{
    //    if (!_isTutorial)
    //        return;


    //    Debug.Log("Computer tutorial " + _tutorialStep);

    //    switch (_tutorialStep)
    //    {
    //        case 1:

    //            EnableDesktopIcons();
    //            break;

    //        case 2:

    //            break;

    //        case 3:
    //            break;

    //        case 4:
    //            break;

    //        case 5:
    //            break;

    //        case 6:
    //            break;

    //        case 7:
    //            break;

    //        case 8:
    //            break;

    //        case 9:
    //            break;


    //    }

    //    _tutorialStep++;

    //}

    public void OpenDesktop()
    {
        GameEvents.OpenDesktop();
        _desktopObject.SetActive(true);
        _inboxObject.SetActive(false);
        _emailObject.SetActive(false);
        _replyObject.SetActive(false);
        _emailNotificationObject.SetActive(false);
        _desktopOn.SetActive(true);
        _advanceStoryOnClose = false;

        if (_isTutorial)
        {
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
        }

    }

    public void CloseDesktop()
    {
        _desktopObject.SetActive(false);

        StartCoroutine(PauseAfterClosingDesktopCoroutine());


        //GameEvents.EndInteraction();
        GameEvents.CloseDesktop();

        if (_isTutorial)
        {
            _desktopOn.SetActive(true);
            //GameEvents.ProgressStory();
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
            _isTutorial = false;
        }

        CheckForNewEmails();

        if (_advanceStoryOnClose)
        {
            GameEvents.ProgressStory();
        }

    }


    public void DoCrash()
    {
        _hasCrashed = true;
        _desktopOn.SetActive(false);
        _emailNotificationObject.SetActive(false);
        _desktopCrash.SetActive(true);
        GameEvents.ProgressStory();
    }
    
    public void FixCrash()
    {
        _hasCrashed = false;
        _desktopOn.SetActive(true);
        _desktopCrash.SetActive(false);
        CheckForNewEmails();
    }



    private IEnumerator PauseAfterClosingDesktopCoroutine()
    {
        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableComputer(bool isTutorial)
    {
        _collider.enabled = true;
        _isTutorial = isTutorial;
        
        if (!isTutorial)
        {
            _desktopOn.SetActive(true);
            EnableDesktopIcons();
            EnableReplyButtonForTutorial();
            EnableSendButtonForTutorial();
            EnableCloseButtonsForTutorial();
            EnableExitDesktop();
        }

    }


    public void DisableComputer()
    {
        _collider.enabled = false;
        _isTutorial = false;
    }


    public void EnableDesktopIcons()
    {
        _desktopIconEmail.enabled = true;
        _desktopIconGames.enabled = true;
    }


    private void OnMouseEnter()
    {
        if (StopInteracting)
            return;

        _highlight.SetActive(true);
    }


    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }


    private void OnMouseDown()
    {
        if (StopInteracting)
            return;

        //if (EventSystem.current.IsPointerOverGameObject())
        //    return;

        //if (DialogueHandler.Instance.IsDialogueOpen)
        //    return;
        //if (TutorialHandler.Instance.IsTutorialOpen)
        //    return;

        _highlight.SetActive(false);
        OpenDesktop();

    }

    public void CheckForNewEmails()
    {
        if (_hasCrashed)
            return;

        var emails = GetEmails(DeskSceneManager.Instance.GameTime);
        if (emails.Count > 0)
        {
            _emailNotificationObject.SetActive(true);
        }
        else
        {
            _emailNotificationObject.SetActive(false);
        }

        _desktopOn.SetActive(true);
        EnableDesktopIcons();
        EnableReplyButtonForTutorial();
        EnableSendButtonForTutorial();
        EnableCloseButtonsForTutorial();
        EnableExitDesktop();

    }

    private List<Email> GetEmails(int requestTime)
    {

        return _emails.Where(w => w.ReceivedTime == requestTime && w.ShouldInclude()).ToList();
    }


    private Email GetEmail(int emailID)
    {
        return _emails.Where(w => w.ID == emailID).Single();
    }

    public void OpenInbox()
    {
        _viewManager.ToggleNavButtons(false);
        _inboxObject.SetActive(true);

        int gameTime = DeskSceneManager.Instance.GameTime;

        _playerEmails = GetEmails(gameTime);

        // load inbox;

        _inboxObject.SetActive(true);

        float emailContainerHeight = 0;

        foreach (GameObject email in _emailButtons)
        {
            Destroy(email);
        }
        _emailButtons.Clear();

        // create buttons for each email
        foreach (Email email in _playerEmails)
        {
            GameObject emailButton = Instantiate(_emailTemplate, _emailContainer.transform);
            emailButton.GetComponent<ClickableEmail>().Init(email.Subject, email.Sender, email.IsRead);
            if (email.IsRead)
            {
                emailButton.GetComponent<Button>().enabled = false;
            }
            else
            {
                emailButton.GetComponent<Button>().onClick.AddListener(() => OpenEmail(emailButton, email));
            }            

            _emailButtons.Add(emailButton);

            emailContainerHeight += emailButton.GetComponent<RectTransform>().sizeDelta.y;

        }

        Vector2 emailContainerDimensions = _emailContainer.GetComponent<RectTransform>().sizeDelta;
        _emailContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(emailContainerDimensions.x, emailContainerHeight);

        if (_isTutorial)
        {
            foreach (var emailBtn in _emailButtons)
            {
                emailBtn.GetComponent<Button>().enabled = false;
            }

            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
        }

    }

    public void EnableEmailsForTutorial()
    {
        foreach (var emailBtn in _emailButtons)
        {
            emailBtn.GetComponent<Button>().enabled = true;
        }
    }

    public void DisableEmailsForTutorial()
    {
        foreach (var emailBtn in _emailButtons)
        {
            emailBtn.GetComponent<Button>().enabled = false;
        }
    }

    public void EnableReplyButtonForTutorial()
    {
        _replyButton.GetComponent<Button>().enabled = true;
    }

    public void EnableSendButtonForTutorial()
    {
        _sendButton.GetComponent<Button>().enabled = true;
    }

    public void EnableCloseButtonsForTutorial()
    {
        foreach (var button in _closeButtons)
        {
            button.enabled = true;
        }
    }


    public void EnableExitDesktop()
    {
        _exitDesktopButton.SetActive(true);
    }

    public void CloseInbox()
    {

        _inboxObject.SetActive(false);

        foreach (GameObject email in _emailButtons)
        {
            Destroy(email);
        }
        _emailButtons.Clear();

        if (_isTutorial)
        {
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
            //_exitDesktopButton.SetActive(true);
        }

    }


    public void OpenEmail(GameObject emailButton, Email email)
    {

        _currentEmailButton = emailButton;
        _currentEmail = email;

        _inboxObject.SetActive(false);
        _emailObject.SetActive(true);

        //_sender.enabled = true;
        _sender.text = _currentEmail.Sender;
        _subject.text = _currentEmail.Subject;
        _emailBody.text = _currentEmail.EmailBody;
        _replyButton.SetActive(_currentEmail.CanReply);
        if (_currentEmail.AdvanceStoryOnClose)
        {
            _advanceStoryOnClose = true;
        }

        if (_isTutorial)
        {
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
        }

    }

    public void ReplyToEmail()
    {
        Debug.Log("replying to email...");

        //_replyButton.SetActive(false);
        //_sendButton.SetActive(true);
        _emailObject.SetActive(false);
        _replyObject.SetActive(true);

        _replySender.enabled = false;
        _replySubject.text = "Re: " + _subject.text;
        _replyBody.text = _currentEmail.ReplyText;

        if (_isTutorial)
        {
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
        }

    }

    public void SendEmail()
    {
        Debug.Log("Reply sent!");

        // do responses
        foreach (var response in _currentEmail.ReplyActions)
        {
            response.DoAction();
        }

        MarkEmailAsRead();

        _inboxObject.SetActive(true);
        _replyObject.SetActive(false);
        
        if (_isTutorial)
        {
            //GameEvents.NextTutorialStep();
            StartCoroutine(PauseBeforeNextTutorial());
        }

    }


    public void CloseReply()
    {

        _emailObject.SetActive(true);
        _replyObject.SetActive(false);

    }

    
    public void CloseEmail()
    {

        MarkEmailAsRead();

        _emailObject.SetActive(false);
        _inboxObject.SetActive(true);

    }

    private void MarkEmailAsRead()
    {
        if (_currentEmail == null)
            return;

        _currentEmail.IsRead = true;
        _currentEmailButton.GetComponent<ClickableEmail>().MarkAsRead();
        _currentEmailButton.GetComponent<Button>().enabled = false;

    }

    public void PlayGame()
    {
        _animator.SetTrigger("PlayingGame");
        Debug.Log("Playing games! woo!");
        PlayerChoices.Instance.ChangePlayerValue("GamesPlayedCount", 1);

    }


    public void EnableVirus()
    {
        _desktopIconVirus.SetActive(true);
    }


    public void ActivateVirus()
    {
        StartCoroutine(ActivateVirusCoroutine());
    }


    private IEnumerator ActivateVirusCoroutine()
    {
        _virusScreenObject.SetActive(true);

        // do animation?
        yield return new WaitForSeconds(5f);
        _virusScreenObject.SetActive(false);
        _desktopIconVirus.SetActive(false);

        GameEvents.ProgressStory();
    }


    private IEnumerator PauseBeforeNextTutorial()
    {
        yield return new WaitForSeconds(0.25f);

        GameEvents.NextTutorialStep();

    }




}
