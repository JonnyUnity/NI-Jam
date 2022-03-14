using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncDecInteract : MonoBehaviour
{
    [SerializeField] private string _choiceName;
    [SerializeField] private int _changeAmount;


    private void OnMouseDown()
    {
        // dialogue stuff...

        ChangePlayerChoiceValue();
    }


    private void ChangePlayerChoiceValue()
    {
        PlayerChoices.Instance.ChangePlayerValue(_choiceName, _changeAmount);
    }


}
