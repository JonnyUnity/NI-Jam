using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private GameObject _desktopObject;

    [SerializeField] private GameObject _desktopOn;
    [SerializeField] private GameObject _desktopCrash;

    [SerializeField] private GameObject _inboxObject;
    [SerializeField] private GameObject _emailObject;
    [SerializeField] private TMP_Text _sender;
    [SerializeField] private TMP_Text _subject;
    [SerializeField] private TMP_Text _emailBody;
    [SerializeField] private GameObject _replyButton;

    [SerializeField] private GameObject _replyObject;
    [SerializeField] private TMP_Text _replySender;
    [SerializeField] private TMP_Text _replySubject;
    [SerializeField] private TMP_Text _replyBody;

    [SerializeField] private GameObject _emailContainer;
    [SerializeField] private GameObject _emailTemplate;

    [SerializeField] private List<Email> _emails;

    private List<GameObject> _emailButtons = new List<GameObject>();
    private List<Email> _playerEmails;

    private Email _currentEmail;
    private GameObject _currentEmailButton;

    private BoxCollider2D _collider;

    private bool _isTutorial = true;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

    }

    private void OnEnable()
    {
        GameEvents.OnCloseDesktop += CloseDesktop;
    }

    private void OnDisable()
    {
        GameEvents.OnCloseDesktop -= CloseDesktop;
    }

    public void OpenDesktop()
    {
        //_viewManager.ToggleNavButtons(false);
        GameEvents.StartInteraction(101);
        GameEvents.OpenDesktop();
        _desktopObject.SetActive(true);
    }

    public void CloseDesktop()
    {
        _desktopObject.SetActive(false);

        StartCoroutine(PauseAfterClosingDesktopCoroutine());
        //StartCoroutine(CloseDesktopCoroutine());


        GameEvents.EndInteraction();

        if (_isTutorial)
        {
            _desktopOn.SetActive(true);
            GameEvents.ProgressStory();
            _isTutorial = false;
        }

    }

    //private IEnumerator CloseDesktopCoroutine()
    //{
        
        
    //}

    private IEnumerator PauseAfterClosingDesktopCoroutine()
    {
        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableComputer()
    {
        _collider.enabled = true;
    }

    private void OnMouseDown()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;
        if (TutorialHandler.Instance.IsTutorialOpen)
            return;

        OpenDesktop();

    }

    private List<Email> GetEmails(int requestTime)
    {

        return _emails.Where(w => w.ReceivedTime <= requestTime && w.ShouldInclude()).ToList();
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

        // create buttons for each email
        foreach (Email email in _playerEmails)
        {
            GameObject emailButton = Instantiate(_emailTemplate, _emailContainer.transform);
            emailButton.GetComponent<ClickableEmail>().Init(email.Subject, email.Sender, email.IsRead);
            emailButton.GetComponent<Button>().onClick.AddListener(() => OpenEmail(emailButton, email));

            _emailButtons.Add(emailButton);

            emailContainerHeight += emailButton.GetComponent<RectTransform>().sizeDelta.y;

        }

        Vector2 emailContainerDimensions = _emailContainer.GetComponent<RectTransform>().sizeDelta;
        _emailContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(emailContainerDimensions.x, emailContainerHeight);

    }


    public void TestOpenEmail()
    {
        Debug.Log("Open sesame!");
    }

    public void CloseInbox()
    {

        _inboxObject.SetActive(false);

        foreach (GameObject email in _emailButtons)
        {
            Destroy(email);
        }
        _emailButtons.Clear();

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

    }

    public void SendEmail()
    {
        Debug.Log("Reply sent!");

        // do responses
        foreach (var response in _currentEmail.ReplyActions)
        {
            response.DoAction();
        }

        CloseReply();
    }


    public void CloseReply()
    {

        _emailObject.SetActive(false);
        _replyObject.SetActive(true);

    }


    public void CloseEmail()
    {

        _currentEmail.IsRead = true;
        _currentEmailButton.GetComponent<ClickableEmail>().MarkAsRead();

        _emailObject.SetActive(false);
        _inboxObject.SetActive(true);

    }

    public void PlayGame()
    {
        Debug.Log("Playing games! woo!");
        PlayerChoices.Instance.ChangePlayerValue("GamesPlayedCount", 1);

    }

}
