using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Medicine : MonoBehaviour
{

    [SerializeField] private GameObject _medsObject;
    [SerializeField] private GameObject _alarmObject;
    [SerializeField] private GameObject _alarmRightObject;

    [SerializeField] private AudioSource _alarmAudio;

    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private List<Interaction> _interactions;
    [SerializeField] private List<Interaction> _pillsInteractions; 

    private BoxCollider2D _alarmCollider;
    private BoxCollider2D _alarmRightCollider;

    private int _medicineID;

    public UnityEvent FinishTakingMedicine;

    private void Awake()
    {
        _alarmCollider = _alarmObject.GetComponent<BoxCollider2D>();
        _alarmRightCollider = _alarmRightObject.GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        GameEvents.OnMedsAlarmStarted += StartAlarm;
        GameEvents.OnStopAlarm += StopAlarm;
        GameEvents.OnPickUpPills += PickUpPills;
        GameEvents.OnDialogueEnded += DialogueEnded;
        FinishTakingMedicine.AddListener(TakenMedicine);
    }



    private void OnDisable()
    {
        GameEvents.OnMedsAlarmStarted += StartAlarm;
        GameEvents.OnStopAlarm -= StopAlarm;
        GameEvents.OnPickUpPills -= PickUpPills;
        GameEvents.OnDialogueEnded -= DialogueEnded;
        FinishTakingMedicine.RemoveListener(TakenMedicine);
    }


    private void DialogueEnded()
    {

        //GameEvents.EndInteraction();
        
    }

    public void TakenMedicine()
    {
        GameEvents.EndInteraction();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAlarm(int alarmID)
    {
        GameEvents.StartInteraction(alarmID);
        _medicineID = alarmID;
        _alarmCollider.enabled = true;
        _alarmRightCollider.enabled = true;
        //_alarmAudio.Play();
        Debug.Log("Meds alarm goes off!");

    }

    private void StopAlarm()
    {
        Debug.Log("Stopped alarm!");
        _alarmCollider.enabled = false;
        _alarmRightCollider.enabled = false;

        List<Dialogue> dialogues = _interactions.Where(w => w.ID == _medicineID).Single().Dialogues;

        DialogueHandler.Instance.StartDialogue(dialogues);

        _medsObject.SetActive(true);

    }


    private void PickUpPills()
    {

        List<Dialogue> dialogues = _pillsInteractions.Where(w => w.ID == _medicineID).Single().Dialogues;
        DialogueHandler.Instance.StartDialogue(dialogues);

    }

}
