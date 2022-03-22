using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickableEmail : MonoBehaviour
{
    [SerializeField] private TMP_Text _subject;
    [SerializeField] private TMP_Text _sender;
    [SerializeField] private GameObject _unreadImage;
    [SerializeField] private GameObject _readImage;

    
    public void Init(string subject, string sender, bool isRead)
    {
        _subject.text = subject;
        _sender.text = sender;
        _unreadImage.SetActive(!isRead);
        _readImage.SetActive(isRead);

    }

    public void MarkAsRead()
    {
        _unreadImage.SetActive(false);
        _readImage.SetActive(true);
    }

}
