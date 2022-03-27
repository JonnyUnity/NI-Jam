using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static event Action OnTutorialStarted;
    public static event Action OnNextTutorialStep;
    public static event Action OnTutorialEnded;

    public static event Action OnDialogueStarted;
    public static event Action<int> OnDialogueEnded;

    public static event Action OnInteractionStart;
    public static event Action OnInteractionEnd;

    public static event Action<int> OnActStart;
    public static event Action OnStoryActionPerformed;
    public static event Action OnActOver;

    public static event Action<int> OnPhoneRings;
    public static event Action<int> OnMedsAlarmStarted;

    // Computer events
    public static event Action OnOpenDesktop;
    public static event Action OnCloseDesktop;

    // Meds Alarm events
    public static event Action<bool> OnStopAlarm;
    public static event Action OnPickUpPills;

    // plant event
    public static event Action OnPlantWatered;
    public static event Action OnWaterCollected;
    public static event Action OnCupUsed;

    public static event Action<int> OnBossStart;
    public static event Action OnBossEnd;

    public static event Action<string> OnSetPlayerFlag;
    public static event Action<string, int> OnChangePlayerValue;


    public static void DialogueStarted()
    {
        OnDialogueStarted?.Invoke();
    }

    public static void DialogueEnded(int interactionObjectID)
    {
        OnDialogueEnded?.Invoke(interactionObjectID);
    }


    public static void StartInteraction()
    {
        OnInteractionStart?.Invoke();
    }


    public static void EndInteraction()
    {
        OnInteractionEnd?.Invoke();
    }


    public static void StartTutorial()
    {
        OnTutorialStarted?.Invoke();
    }

    public static void NextTutorialStep()
    {
        OnNextTutorialStep?.Invoke();
    }

    public static void TutorialFinished()
    {
        OnTutorialEnded?.Invoke();
    }

    public static void ProgressStory()
    {
        OnStoryActionPerformed?.Invoke();
    }


    public static void StartAct(int actNumber)
    {
        OnActStart?.Invoke(actNumber);
    }

    public static void ActOver()
    {
        OnActOver?.Invoke();
    }


    public static void StartBoss(int actNumber)
    {
        OnBossStart?.Invoke(actNumber);
    }

    public static void EndBoss()
    {
        OnBossEnd?.Invoke();
    }


    public static void UseCup()
    {
        OnCupUsed?.Invoke();
    }


    public static void WaterCollected()
    {
        OnWaterCollected?.Invoke();
    }

    public static void WaterPlant()
    {
        OnPlantWatered?.Invoke();
    }

    public static void PhoneRings(int callID)
    {
        OnPhoneRings?.Invoke(callID);
    }

    public static void MedsAlarmGoesOff(int alarmID)
    {
        OnMedsAlarmStarted?.Invoke(alarmID);
    }

    public static void PickUpPills()
    {
        OnPickUpPills?.Invoke();
    }


    public static void OpenDesktop()
    {
        OnOpenDesktop?.Invoke();
    }


    public static void CloseDesktop()
    {
        OnCloseDesktop?.Invoke();
    }


    public static void StopAlarm(bool isRight)
    {
        OnStopAlarm?.Invoke(isRight);
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
