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

    [SerializeField] private GameObject _inboxObject;
    [SerializeField] private GameObject _emailObject;
    [SerializeField] private TMP_Text _sender;
    [SerializeField] private TMP_Text _subject;
    [SerializeField] private TMP_Text _emailBody;
    [SerializeField] private GameObject _replyButton;
    [SerializeField] private GameObject _sendButton;

    [SerializeField] private GameObject _emailContainer;
    [SerializeField] private GameObject _emailTemplate;

    [SerializeField] private List<Email> _emails;

    private List<GameObject> _emailButtons = new List<GameObject>();
    private List<Email> _playerEmails;
    private Email _currentEmail;

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

        //int gameTime = GameManager.Instance.GetGameTime();
        int gameTime = 100;

        _playerEmails = GetEmails(gameTime);

        // load inbox;

        _inboxObject.SetActive(true);

        float emailContainerHeight = 0;

        // create buttons for each email
        foreach (Email email in _playerEmails)
        {
            GameObject emailButton = Instantiate(_emailTemplate, _emailContainer.transform);
            emailButton.GetComponent<ClickableEmail>().Init(email.Subject, email.Sender);
            emailButton.GetComponent<Button>().onClick.AddListener(() => OpenEmail(email));

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

        _viewManager.ToggleNavButtons(true);
    }


    public void OpenEmail(Email email)
    {

        _currentEmail = email;

        _inboxObject.SetActive(false);
        _emailObject.SetActive(true);

        _sender.enabled = true;
        _sender.text = _currentEmail.Sender;
        _subject.text = _currentEmail.Subject;
        _emailBody.text = _currentEmail.EmailBody;
        _replyButton.SetActive(_currentEmail.CanReply);

    }

    public void ReplyToEmail()
    {
        Debug.Log("replying to email...");

        _replyButton.SetActive(false);
        _sendButton.SetActive(true);

        _sender.enabled = false;
        _subject.text = "Re: " + _subject.text;
        _emailBody.text = _currentEmail.ReplyText;

    }

    public void SendEmail()
    {
        Debug.Log("Reply sent!");

        _emailObject.SetActive(false);
        _inboxObject.SetActive(true);

        // do responses
        foreach (var response in _currentEmail.ReplyActions)
        {
            response.DoAction();
        }

    }


    public void CloseEmail()
    {

        _currentEmail.Read = true;

        _emailObject.SetActive(false);
        _inboxObject.SetActive(true);

    }

    public void PlayGame()
    {
        Debug.Log("Playing games! woo!");
        PlayerChoices.Instance.ChangePlayerValue("GamesPlayedCount", 1);

    }

}
