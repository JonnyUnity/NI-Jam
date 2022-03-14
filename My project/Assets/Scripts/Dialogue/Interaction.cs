using System;
using System.Collections.Generic;

[Serializable]
public class Interaction
{
    public int ID;
    public List<Dialogue> Dialogues;
    public List<Response> Responses;

}
