using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{

    private static readonly int INTERACTIONOBJECT_ID = 8;

    [SerializeField] private Interaction _act1Interaction;

    [SerializeField] private Interaction _lizardmanAct2Interaction;
    [SerializeField] private Interaction _hitmanAct2Interaction;
    [SerializeField] private Interaction _clonesAct2Interaction;

    [SerializeField] private Interaction _lizardmanFinale;
    [SerializeField] private Interaction _hitmanFinale;
    [SerializeField] private Interaction _clonesFinale;

    [SerializeField] private AudioClip _bossVoiceClip;
    [SerializeField] private Animator _animator;

    private AudioSource _backgroundAudio;


    private void Awake()
    {
        _backgroundAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // debug
        //_backgroundAudio.Play();

        //StartBossInteraction(1);
    }

    private void OnEnable()
    {

        GameEvents.OnBossStart += StartBossInteraction;
        GameEvents.OnDialogueEnded += EndBossInteraction;
        
    }

    private void OnDisable()
    {
        GameEvents.OnBossStart -= StartBossInteraction;
        GameEvents.OnDialogueEnded -= EndBossInteraction;
    }


    private void StartBossInteraction(int actNumber)
    {


        _backgroundAudio.Play();
        Interaction thisInteraction = null;

        switch (actNumber)
        {
            case 1:

                thisInteraction = _act1Interaction;
                break;

            case 2:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    thisInteraction = _lizardmanAct2Interaction;
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    thisInteraction = _hitmanAct2Interaction;
                }
                else
                {
                    thisInteraction = _clonesAct2Interaction;
                }
                break;

            case 3:

                if (PlayerChoices.Instance.OnLizardmanStory)
                {
                    thisInteraction = _lizardmanFinale;
                }
                else if (PlayerChoices.Instance.OnHitmanStory)
                {
                    thisInteraction = _hitmanFinale;
                }
                else
                {
                    thisInteraction = _clonesFinale;
                }
                break;
        }

        DialogueHandler.Instance.StartDialogue(thisInteraction.Dialogues, _bossVoiceClip, INTERACTIONOBJECT_ID);

    }

    public void EndBossInteraction(int interactionObjectID)
    {
        if (interactionObjectID != INTERACTIONOBJECT_ID)
            return;

        StartCoroutine(FadeOutCoroutine());
       
    }

    private IEnumerator FadeOutCoroutine()
    {
        _animator.SetTrigger("LeaveOffice");
        yield return new WaitForSecondsRealtime(1.5f);
        GameEvents.EndBoss();
    }

}
