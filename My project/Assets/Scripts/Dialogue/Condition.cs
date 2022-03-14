using System;
using UnityEngine;

[Serializable]
public class Condition
{
    public string _choice;
    public ConditionTypeEnum _type;
    public int _requiredValue;

    public bool IsMet()
    {
        
        switch (_type)
        {
            case ConditionTypeEnum.Flag:

                return PlayerChoices.Instance.GetPlayerFlagValue(_choice);

            case ConditionTypeEnum.Value:

                int value = PlayerChoices.Instance.GetPlayerCountValue(_choice);
                return (value >= _requiredValue);

            default:
                break;
        }

        return true;
    }


}

public enum ConditionTypeEnum
{
    Flag,
    Value
}
