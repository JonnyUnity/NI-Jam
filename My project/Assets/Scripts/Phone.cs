using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Phone : MonoBehaviour
{
    private static readonly int INTERACTIONOBJECT_ID = 1;

    [SerializeField] private Sprite _phoneOnHook;
    [SerializeField] private Sprite _phoneOffHook;
    [SerializeField] private GameObject _highlight;
    private Animator _animator;

    [SerializeField] private AudioSource _phoneAudio;
    [SerializeField] private AudioClip _phoneRingClip;
    [SerializeField] private AudioClip _bossRingClip;
    [SerializeField] private AudioClip _pickUpReceiverClip;
    [SerializeField] private AudioClip _voiceOnPhoneClip;

    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private List<Interaction> _calls;

    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;
    private int _callID;
    private bool _isReceiverDown;
    
    private bool StopInteracting => (EventSystem.current.IsPointerOverGameObject() ||  DialogueHandler.Instance.IsDialogueOpen);


    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
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
        _animator.SetBool("Ringing", true);
        _phoneAudio.Play();
    }

    public void EnablePhone()
    {
        _collider.enabled = true;
    }

    public void ReceivePhoneCall(int callID)
    {
        _callID = callID;
        EnablePhone();
        if (!_animator.GetBool("Ringing"))
        {
            _animator.SetBool("Ringing", true);
        }        

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
        _animator.SetBool("BossRinging", true);
        _phoneAudio.clip = _bossRingClip;
        _phoneAudio.Play();
    }


    public void FinishPhoneCall()
    {
        _phoneAudio.PlayOneShot(_pickUpReceiverClip);
        HangUp(INTERACTIONOBJECT_ID);
        //_renderer.sprite = _phoneOnHook;
        //_collider.enabled = false;
    }

    // Stops phone ringing
    public void HangUp(int interactionObjectID)
    {
        if (interactionObjectID != INTERACTIONOBJECT_ID)
            return;

        //if (_isReceiverDown)
        //    return;

        //if (_phoneAudio.isPlaying)
        //{
        //    _phoneAudio.Stop();
        //}

        _phoneAudio.PlayOneShot(_pickUpReceiverClip);
        _renderer.sprite = _phoneOnHook;
        _collider.enabled = false;
        GameEvents.EndInteraction();
    }


    private void OnMouseOver()
    {
        if (StopInteracting)
            return;

        _highlight.SetActive(true);
    }

    
    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }


    public void OnMouseDown()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //    return;
        //if (DialogueHandler.Instance.IsDialogueOpen)
        //    return;

        if (StopInteracting)
            return;

        if (!_phoneAudio.isPlaying)
            return;

        _highlight.SetActive(false);

        //if (_isReceiverDown)
        //{
         GameEvents.StartInteraction();

        _animator.SetBool("Ringing", false);
        _animator.SetBool("BossRinging", false);
        _phoneAudio.Stop();
        _phoneAudio.PlayOneShot(_pickUpReceiverClip);
        _collider.enabled = false;

        _renderer.sprite = _phoneOffHook;

        var dialogues = _calls.Where(w => w.ID == _callID).Single().Dialogues;

        // do dialogue...
        DialogueHandler.Instance.StartDialogue(dialogues, _voiceOnPhoneClip, INTERACTIONOBJECT_ID);

        //}
        //else
        //{
        //    _renderer.sprite = _phoneOnHook;
        //    _collider.enabled = false;

        //}

        //_isReceiverDown = !_isReceiverDown;
       
    }

}
