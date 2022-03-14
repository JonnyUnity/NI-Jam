using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static event Action OnDialogueStarted;
    public static event Action OnDialogueEnded;

    public static event Action<int> OnActivateObject;

    public static event Action<string> OnSetPlayerFlag;
    public static event Action<string, int> OnChangePlayerValue;


    public static void DialogueStarted()
    {
        OnDialogueStarted?.Invoke();
    }

    public static void DialogueEnded()
    {
        OnDialogueEnded?.Invoke();
    }


    public static void ActivateObject(int objectID)
    {
        OnActivateObject?.Invoke(objectID);
    }
        


    public static void SetPlayerFlag(string propertyName)
    {
        OnSetPlayerFlag?.Invoke(propertyName);
    }

    public static void ChangePlayerValue(string propertyName, int changeInValue)
    {
        OnChangePlayerValue?.Invoke(propertyName, changeInValue);
    }

}
