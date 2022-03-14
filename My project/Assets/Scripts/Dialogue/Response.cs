using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

[Serializable]
public class Response
{
    public int ID;
    public string Answer;

    public List<Condition> Conditions;

    public ResponseAction Action;

    public UnityEvent<int> OnSelect;

    public bool IncludeResponse()
    {
        if (Conditions.Count() == 0)
            return true;

         return Conditions.All(x => x.IsMet());

    }


}
