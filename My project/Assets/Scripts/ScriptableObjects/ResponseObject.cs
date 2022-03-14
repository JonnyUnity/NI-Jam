using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Response", menuName = "NIJam/Response")]
public class ResponseObject : ScriptableObject
{
    public string Text;
    public object Value;
    public DialogueObject FollowUpDialogue;

    public UnityEvent OnSelect;

}
