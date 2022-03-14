using System;

[Serializable]
public class Dialogue
{
    public int ID;
    public string Speaker;
    public string[] Sentences;

    public DialogueNode Links;

}
