using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoWorkerInteraction : MonoBehaviour
{
    private int _interactionID;

    [SerializeField] private GameObject _coWorkerObject;

    [SerializeField] private DeskView _view; 
    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private List<Interaction> _interactions;

    private void OnEnable()
    {

        GameEvents.OnDialogueEnded += DialogueEnded;
        
    }


    private void OnDisable()
    {
        GameEvents.OnDialogueEnded -= DialogueEnded;
    }


    private void DialogueEnded()
    {

        _coWorkerObject.SetActive(false);
        GameEvents.EndInteraction();

    }

    public void DoTutorial(int interactionID)
    {
        _interactionID = interactionID;
        _coWorkerObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (DialogueHandler.Instance.IsDialogueOpen)
            return;
        if (TutorialHandler.Instance.IsTutorialOpen)
            return;

        GameEvents.StartInteraction(_interactionID);
        ChatWithWorker();

    }

    public void DoVisit(int interactionID)
    {
        _viewManager.GoToView(_view);
        _interactionID = interactionID;
        GameEvents.StartInteraction(_interactionID);

        ChatWithWorker();
    }

    private void ChatWithWorker()
    {
        _coWorkerObject.SetActive(true);
        List<Dialogue> dialogues = _interactions.Where(w => w.ID == _interactionID).Single().Dialogues;

        DialogueHandler.Instance.StartDialogue(dialogues);

    }
}
