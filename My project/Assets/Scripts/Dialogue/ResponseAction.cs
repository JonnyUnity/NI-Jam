using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponseAction
{
    public string choiceFlagName;
    public int value;
    public ResponseActionTypeEnum Type;


    public void DoAction()
    {
        switch (Type)
        {
            case ResponseActionTypeEnum.GoToDialogue:

                Debug.Log("DialogueHandler start dialogue with id: " + value);
                DialogueHandler.Instance.ContinueDialogue(value);
                break;

            case ResponseActionTypeEnum.SetFlag:

                Debug.Log("Setting Player Choices flag: " + choiceFlagName);
                PlayerChoices.Instance.SetPlayerFlag(choiceFlagName);
                break;

            case ResponseActionTypeEnum.UpdateCount:

                Debug.Log($"Updating Player Choice {choiceFlagName} count by: {value}");
                PlayerChoices.Instance.ChangePlayerValue(choiceFlagName, value);
                break;

            case ResponseActionTypeEnum.AdvanceStory:

                Debug.Log("Advancing story...");
                GameEvents.ProgressStory();
                break;

            case ResponseActionTypeEnum.EndDialogue:

                Debug.Log("Ending dialogue");
                GameEvents.DialogueEnded(value);
                break;

            case ResponseActionTypeEnum.WaterPlant:

                Debug.Log("Watering plant!");
                GameEvents.WaterPlant();
                break;

            default:
                Debug.Log("Type not set!");
                break;
        }

    }

}

public enum ResponseActionTypeEnum
{
    None,
    GoToDialogue,
    SetFlag,
    UpdateCount,
    AdvanceStory,
    EndDialogue,
    WaterPlant
}
