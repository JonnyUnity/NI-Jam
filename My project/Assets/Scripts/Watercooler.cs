using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Watercooler : MonoBehaviour
{

    private bool _isTutorial = true;

    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void EnableWaterCooler()
    {
        _collider.enabled = true;
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (DialogueHandler.Instance.IsDialogueOpen)
            return;
        if (TutorialHandler.Instance.IsTutorialOpen)
            return;

        Debug.Log("Used water cooler!");

        if (_isTutorial)
        {

            _isTutorial = false;
            GameEvents.ProgressStory();
        }

        _collider.enabled = false;
        GameEvents.WaterCollected();

    }


}
