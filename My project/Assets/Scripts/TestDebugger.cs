using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebugger : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Text _debugText;
    [SerializeField] private DeskSceneManager _deskSceneManager;

    
    public void AdvanceStoryBeat()
    {
        GameEvents.ProgressStory();
    }

    public void UpdateDebugText(string text)
    {
        _debugText.text = text;
    }

}
