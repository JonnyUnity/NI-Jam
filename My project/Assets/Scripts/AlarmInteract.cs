using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlarmInteract : MonoBehaviour
{
    [SerializeField] private bool _isRight;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;


        GameEvents.StopAlarm(_isRight);

    }

}
