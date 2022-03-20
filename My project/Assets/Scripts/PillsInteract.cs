using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PillsInteract : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;


        GameEvents.PickUpPills();

    }
}
