using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FullCup : MonoBehaviour
{

    private static readonly int INTERACTIONOBJECT_ID = 7;

    [SerializeField] private GameObject _highlight;

    [SerializeField] private Interaction _tutorialInteraction;
    [SerializeField] private Interaction _useWaterInteraction;

    private BoxCollider2D _collider;
    private bool _isTutorial;

    private bool StopInteracting => (EventSystem.current.IsPointerOverGameObject() || DialogueHandler.Instance.IsDialogueOpen);
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
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

        gameObject.SetActive(false);
        if (!_isTutorial)
        {
            GameEvents.EndInteraction();
        }        
    }

    public void UseTutorialDialogue()
    {
        _isTutorial = true;
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


    private void OnMouseDown()
    {
        if (StopInteracting)
            return;

        //if (EventSystem.current.IsPointerOverGameObject())
        //    return;
        //if (DialogueHandler.Instance.IsDialogueOpen)
        //    return;

        Debug.Log("Used cup!");
        GameEvents.StartInteraction();
        GameEvents.UseCup();
        
        //gameObject.SetActive(false);
        _collider.enabled = false;

        if (_isTutorial)
        {
            DialogueHandler.Instance.StartDialogue(_tutorialInteraction.Dialogues, null, INTERACTIONOBJECT_ID);
            _isTutorial = false;
        }
        else
        {
            DialogueHandler.Instance.StartDialogue(_useWaterInteraction.Dialogues, null, INTERACTIONOBJECT_ID);
        }

    }

}
