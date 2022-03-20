using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Interaction", menuName = "Office Secrets/Interaction")]
public class Interaction : ScriptableObject
{
#if UNITY_EDITOR
    public string Name;
#endif

    public int ID;
    public List<Dialogue> Dialogues;


}
