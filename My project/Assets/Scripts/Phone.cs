using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Phone : MonoBehaviour
{

    [SerializeField] private Sprite _phoneOnHook;
    [SerializeField] private Sprite _phoneOffHook;
    [SerializeField] private AudioSource _phoneAudio;
    [SerializeField] private AudioClip _phoneRingClip;
    [SerializeField] private AudioClip _bossRingClip;
    [SerializeField] private AudioClip _pickUpReceiverClip;

    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private List<Interaction> _calls;

    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;

    private int _callID;
    private bool _isReceiverDown = true;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }


    private void OnEnable()
    {
        GameEvents.OnPhoneRings += ReceivePhoneCall;
        GameEvents.OnDialogueEnded += HangUp;
    }


    private void OnDisable()
    {
        GameEvents.OnPhoneRings -= ReceivePhoneCall;
        GameEvents.OnDialogueEnded -= HangUp;
    }

    public void PhoneTutorial()
    {
        _phoneAudio.clip = _phoneRingClip;
        _phoneAudio.Play();
    }

    public void ReceivePhoneCall(int callID)
    {
        _callID = callID;
        _collider.enabled = true;
        if (!_phoneAudio.isPlaying)
        {
            _phoneAudio.clip = _phoneRingClip;
            _phoneAudio.Play();

        }
    }

    public void ReceiveBossPhoneCall(int callID)
    {
        _callID = callID;
        _collider.enabled = true;
        _phoneAudio.clip = _bossRingClip;
        _phoneAudio.Play();
    }


    public void FinishPhoneCall()
    {
        _phoneAudio.PlayOneShot(_pickUpReceiverClip);
        HangUp();
        //_renderer.sprite = _phoneOnHook;
        //_collider.enabled = false;
    }

    // Stops phone ringing
    public void HangUp()
    {
        if (_isReceiverDown)
            return;

        if (_phoneAudio.isPlaying)
        {
            _phoneAudio.Stop();
        }
        _renderer.sprite = _phoneOnHook;
        _collider.enabled = false;
        GameEvents.EndInteraction();
    }

    private void Update()
    {
        
        // option for phone to ring out?


    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;
        if (TutorialHandler.Instance.IsTutorialOpen)
            return;


        if (!_phoneAudio.isPlaying && _isReceiverDown)
            return;

        if (_isReceiverDown)
        {
            GameEvents.StartInteraction(_callID);
            _phoneAudio.Stop();
            _phoneAudio.PlayOneShot(_pickUpReceiverClip);

            _renderer.sprite = _phoneOffHook;

            var dialogues = _calls.Where(w => w.ID == _callID).Single().Dialogues;

            // do dialogue...
            DialogueHandler.Instance.StartDialogue(dialogues);

        }
        else
        {
            _renderer.sprite = _phoneOnHook;
            _collider.enabled = false;

        }

        _isReceiverDown = !_isReceiverDown;

        
    }

}
