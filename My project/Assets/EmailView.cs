using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailView : MonoBehaviour
{

    [SerializeField] private ScrollRect _emailView;
    [SerializeField] private float _scrollSpeed;


    public void ScrollUp()
    {
        _emailView.verticalNormalizedPosition = Mathf.Min(_emailView.verticalNormalizedPosition + _scrollSpeed, 1);
    }

    public void ScrollDown()
    {
        _emailView.verticalNormalizedPosition = Mathf.Max(_emailView.verticalNormalizedPosition - _scrollSpeed, 0);
    }

}
