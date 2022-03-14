using System;

[Serializable]
public class DialogueNode
{
    public DialogueNodeType Type;
    public int[] IDs;
}


public enum DialogueNodeType
{
    None,
    Dialogue,
    Response,
    Action // something outside of dialogue happens...
}