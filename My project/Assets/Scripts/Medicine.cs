using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Medicine : MonoBehaviour
{
    private static readonly int ALARMOBJECT_ID = 4;
    private static readonly int PILLSOBJECT_ID = 6;

    [SerializeField] private GameObject _medsObject;
    [SerializeField] private GameObject _alarmObject;
    [SerializeField] private GameObject _alarmRightObject;

    private Animator _alarmAnimator;
    [SerializeField] private AudioSource _alarmAudio;

    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private List<Interaction> _interactions;
    [SerializeField] private List<Interaction> _pillsInteractions; 

    private BoxCollider2D _alarmCollider;
    private BoxCollider2D _alarmRightCollider;
    private BoxCollider2D _medsCollider;

    private int _medicineID;
    private bool _stoppedRightAlarm;


    private void Awake()
    {
        _alarmCollider = _alarmObject.GetComponent<BoxCollider2D>();
        _alarmRightCollider = _alarmRightObject.GetComponent<BoxCollider2D>();
        _alarmAnimator = _alarmObject.GetComponent<Animator>();

        _medsCollider = _medsObject.GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        GameEvents.OnMedsAlarmStarted += StartAlarm;
        GameEvents.OnStopAlarm += StopAlarm;
        GameEvents.OnPickUpPills += PickUpPills;
        GameEvents.OnDialogueEnded += DialogueEnded;
    }


    private void OnDisable()
    {
        GameEvents.OnMedsAlarmStarted += StartAlarm;
        GameEvents.OnStopAlarm -= StopAlarm;
        GameEvents.OnPickUpPills -= PickUpPills;
        GameEvents.OnDialogueEnded -= DialogueEnded;
    }


    private void DialogueEnded(int interactionObjectID)
    {
        if (interactionObjectID == ALARMOBJECT_ID || interactionObjectID == PILLSOBJECT_ID)
        {
            if (_stoppedRightAlarm)
            {
                _viewManager.LookLeft();
                _viewManager.ToggleNavButtons(false);
                _stoppedRightAlarm = false;
            }

            if (interactionObjectID == PILLSOBJECT_ID)
            {
                Debug.Log("Finished taking pills or not");
                GameEvents.EndInteraction();
            }

        }        
    }


    public void StartAlarm(int alarmID)
    {
        GameEvents.StartInteraction();

        _viewManager.GoToView(DeskView.MIDDLE);
        _viewManager.ToggleNavButtons(false);

        _medicineID = alarmID;
        _alarmCollider.enabled = true;
        _alarmRightCollider.enabled = true;
        _alarmAudio.Play();
        _alarmAnimator.SetBool("Activate", true);
        Debug.Log("Meds alarm goes off!");

    }

    private void StopAlarm(bool isRight)
    {
        GameEvents.StartInteraction();
        _stoppedRightAlarm = isRight;

        Debug.Log("Stopped alarm!");
        _alarmCollider.enabled = false;
        _alarmRightCollider.enabled = false;
        _alarmAudio.Stop();
        _alarmAnimator.SetBool("Activate", false);

        List<Dialogue> dialogues = _interactions.Where(w => w.ID == _medicineID).Single().Dialogues;

        DialogueHandler.Instance.StartDialogue(dialogues, null, ALARMOBJECT_ID);

        _medsCollider.enabled = true;

    }


    private void PickUpPills()
    {

        List<Dialogue> dialogues = _pillsInteractions.Where(w => w.ID == _medicineID).Single().Dialogues;
        DialogueHandler.Instance.StartDialogue(dialogues, null, PILLSOBJECT_ID);
        _medsCollider.enabled = false;

    }

}
