using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Office Secrets/Dialogue")]
public class Dialogue : ScriptableObject
{
    public int ID;
    public string Speaker;
    [TextArea] public string[] Sentences;

    public List<Response> Responses;

    public DialogueNode Links;

    public bool AdvanceStoryOnClose;

    public bool HasResponses => (Responses != null && Responses.Count > 0);
    
}
