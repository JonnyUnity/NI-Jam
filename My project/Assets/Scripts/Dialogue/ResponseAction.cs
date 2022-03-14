using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponseAction
{
    public string Choice;
    public ResponseActionTypeEnum Type;
    public int ChangeInValue;
    public int DialogueID;


}

public enum ResponseActionTypeEnum
{
    None,
    SetFlag,
    UpdateCount
}
