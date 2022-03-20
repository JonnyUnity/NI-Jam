using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Response", menuName = "Office Secrets/Response")]
public class Response : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _text;
    [SerializeField] [TextArea] private string[] _answerSentences;
    [SerializeField] private List<Condition> _conditions;
    [SerializeField] private List<ResponseAction> _actions;
    [SerializeField] private UnityEvent _onSelect;

    public int ID => _id;
    public string Text => _text;
    public string[] AnswerSentences => _answerSentences;
    public List<Condition> Conditions => _conditions;
    public List<ResponseAction> Actions => _actions;
    public UnityEvent OnSelect => _onSelect;

    public bool IncludeResponse()
    {
        if (Conditions.Count() == 0)
            return true;

        return Conditions.All(x => x.IsMet());

    }

}
