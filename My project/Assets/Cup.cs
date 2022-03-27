using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cup : MonoBehaviour
{

    private static readonly int INTERACTIONOBJECT_ID = 7;

    [SerializeField] private GameObject _fullCupObject;

    private BoxCollider2D _collider;
    private bool _previousState;

    private void Awake()
    {
        _collider = _fullCupObject.GetComponent<BoxCollider2D>();
        GameEvents.OnWaterCollected += FillCup;
        GameEvents.OnCupUsed += EmptyCup;
        GameEvents.OnDialogueEnded += DialogueEnded;
    }



    private void OnDisable()
    {
        GameEvents.OnWaterCollected -= FillCup;
        GameEvents.OnCupUsed -= EmptyCup;
        GameEvents.OnDialogueEnded -= DialogueEnded;
    }


    private void DialogueEnded(int interactionObjectID)
    {
        if (interactionObjectID != INTERACTIONOBJECT_ID)
            return;

        Debug.Log("Cup Dialogue ended!");
        GameEvents.EndInteraction();
    }

    private void FillCup()
    {
        _fullCupObject.SetActive(true);
        _collider.enabled = true;
    }

    private void EmptyCup()
    {
        _fullCupObject.SetActive(false);
        _collider.enabled = false;
    }


    public void EnableCup()
    {
        _collider.enabled = _previousState;
    }


    public void DisableCup()
    {
        _previousState = _collider.enabled;
        _collider.enabled = false;
    }


    //private void OnMouseOver()
    //{
    //    _highlight.SetActive(true);
    //}


    //private void OnMouseExit()
    //{
    //    _highlight.SetActive(false);
    //}


    //private void OnMouseDown()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //        return;
    //    if (DialogueHandler.Instance.IsDialogueOpen)
    //        return;


    //    Debug.Log("Used cup!");


    //    _fullCupObject.SetActive(false);
    //    _collider.enabled = false;

    //}

}
