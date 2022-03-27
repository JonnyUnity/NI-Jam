using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PillsInteract : MonoBehaviour
{

    [SerializeField] private GameObject _highlight;

    private bool StopInteracting => EventSystem.current.IsPointerOverGameObject() || DialogueHandler.Instance.IsDialogueOpen;

    private void OnMouseEnter()
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

        _highlight.SetActive(false);
        GameEvents.PickUpPills();

    }
}
