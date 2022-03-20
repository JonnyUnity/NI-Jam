using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoices
{
    static PlayerChoices _instance = null;
    static readonly object _padlock = new object();

    public static int WateredCount { get; private set; }
    public int GamesPlayedCount { get; private set; }
    
    public static bool TakenMeds { get; private set; }
    public bool TalkedDiesel { get; private set; }
    public bool WatchedGameLastNight { get; private set; }
    
    // EMAIL FLAGS
    public bool RepliedToCharlieEmail { get; private set; }
    public bool RepliedToJeremyEmail { get; private set; }
    public bool RepliedToJeremy2Email { get; private set; }
    public bool RepliedToJeremy3Email { get; private set; }


    // STORYLINE FLAGS
    public bool OnLizardmanStory { get; private set; }
    public bool OnHitmanStory { get; private set; }
    public bool OnClonesStory { get; private set; }


    //public PlayerChoices()
    //{
    //    GameEvents.OnSetPlayerFlag += SetPlayerFlag;
    //    GameEvents.OnChangePlayerValue += ChangePlayerValue;

    //}

    public static PlayerChoices Instance
    {
        get
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new PlayerChoices();
                }

                return _instance;
            }
        }
    }


    //public int WateredPlant()
    //{
    //    WateredCount++;
    //    return WateredCount;
    //}


    //public void TakeMeds()
    //{
    //    TakenMeds = true;
    //}


    //public void MissedMeds()
    //{
    //    _medsLevel--;
    //}


    public void SetPlayerFlag(string propertyName)
    {
        var prop = GetType().GetProperty(propertyName);
        if (prop != null)
        {
            prop.SetValue(this, true);
            Debug.Log($"Set flag: { propertyName} to true");
        }
                
    }

    public void ChangePlayerValue(string propertyName, int changeInValue)
    {
        var prop = GetType().GetProperty(propertyName);
        if (prop != null)
        {
            int currValue = (int)prop.GetValue(this);
            prop.SetValue(this, currValue + changeInValue);
            Debug.Log($"Change property: {propertyName} value from {currValue} to {currValue + changeInValue}");

            int value = GetPlayerCountValue(propertyName);
            Debug.Log("New value = " + value);

        }                
    }


    public bool GetPlayerFlagValue(string propertyName)
    {
        var prop = GetType().GetProperty(propertyName);
        bool value = (bool)prop.GetValue(this);

        return value;
    }

    public int GetPlayerCountValue(string propertyName)
    {
        var prop = GetType().GetProperty(propertyName);
        int value = (int)prop.GetValue(this);

        return value;
    }


}
