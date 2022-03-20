using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickableEmail : MonoBehaviour
{
    [SerializeField] private TMP_Text _subject;
    [SerializeField] private TMP_Text _sender;


    
    public void Init(string subject, string sender)
    {
        _subject.text = subject;
        _sender.text = sender;
    }

}
