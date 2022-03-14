using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteract : MonoBehaviour
{

    [SerializeField] private string _choiceName;
    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private List<Response> _responses;
    
    private bool _alreadyInteracted;

    private void OnMouseDown()
    {
        if (_alreadyInteracted)
            return;

        if (_dialogues != null)
        {
            DialogueHandler.Instance.StartDialogue(_dialogues, _responses);
        }

        _alreadyInteracted = true;

    }


    public void SetChoiceFlag(int choice)
    {
        GameEvents.SetPlayerFlag(_choiceName);
        _alreadyInteracted = true;
    }

    public void DialogResult(int choice)
    {
        switch (choice)
        {
            case 0:
                Debug.Log("close!");
                break;
            case 1:
                SetChoiceFlag(1);
                break;
        }
    }


}
