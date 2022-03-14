using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "NIJam/Dialogue")]
public class DialogueObject : ScriptableObject
{
    public string Speaker;
    public string[] Text;

    public ResponseObject[] Responses;

}
