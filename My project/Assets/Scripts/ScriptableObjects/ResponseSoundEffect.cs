using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Response Sound Effect", menuName = "Office Secrets/Response Sound Effect")]
public class ResponseSoundEffect : ScriptableObject
{

    [SerializeField] private AudioClip _clip;

    public void PlaySound()
    {
        DialogueHandler.Instance.PlayResponseSoundEffect(_clip);
    }

}
