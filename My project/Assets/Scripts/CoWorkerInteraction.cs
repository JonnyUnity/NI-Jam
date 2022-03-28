using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoWorkerInteraction : MonoBehaviour
{
    private static readonly int INTERACTIONOBJECT_ID = 2;

    private int _interactionID;

    [SerializeField] private GameObject _coWorkerObject;
    [SerializeField] private GameObject _conversationAlertObject;
    [SerializeField] private GameObject _lizardVersion;

    [SerializeField] private DeskView _view; 
    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private List<Interaction> _interactions;
    [SerializeField] private AudioClip _voiceClip;

    private void OnEnable()
    {

        GameEvents.OnDialogueEnded += DialogueEnded;
        
    }


    private void OnDisable()
    {
        GameEvents.OnDialogueEnded -= DialogueEnded;
    }


    private void DialogueEnded(int interactionObjectID)
    {
        if (interactionObjectID != INTERACTIONOBJECT_ID)
            return;

        _coWorkerObject.SetActive(false);
        _conversationAlertObject.SetActive(false);
        GameEvents.EndInteraction();
        
    
    }

    public void DoTutorial(int interactionID)
    {
        _interactionID = interactionID;
        _coWorkerObject.SetActive(true);
        ShowConversationAlert();
    }

    public void ShowConversationAlert()
    {
        _conversationAlertObject.SetActive(true);
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (DialogueHandler.Instance.IsDialogueOpen)
            return;
        //if (TutorialHandler.Instance.IsTutorialOpen)
        //    return;

        //GameEvents.StartInteraction();
        ChatWithWorker();

    }

    public void StartGossip(int interactionID, bool isLizard)
    {
        ShowConversationAlert();
        _interactionID = interactionID;
        _coWorkerObject.SetActive(true);
        _lizardVersion.SetActive(isLizard);

    }

    public void DoVisit(int interactionID, bool isLizard)
    {
         _viewManager.GoToView(_view);
        _interactionID = interactionID;
        _lizardVersion.SetActive(isLizard);

        ChatWithWorker();
    }

    private void ChatWithWorker()
    {
        GameEvents.StartInteraction();
        _coWorkerObject.SetActive(true);
        List<Dialogue> dialogues = _interactions.Where(w => w.ID == _interactionID).Single().Dialogues;

        DialogueHandler.Instance.StartDialogue(dialogues, _voiceClip, INTERACTIONOBJECT_ID);

    }
}
