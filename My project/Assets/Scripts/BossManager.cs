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
    [SerializeField] private Interaction _hitmanBobDiesFinale;
    [SerializeField] private Interaction _hitmanBobLivesFinale;
    [SerializeField] private Interaction _clonesFinale;

    [SerializeField] private AudioClip _bossVoiceClip;
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _chairObject;

    // Clones Ending
    [SerializeField] private GameObject[] _bobsObjects;

    // Lizardman Ending
    [SerializeField] private GameObject _lizardBossObject;


    // Hitman Ending
    [SerializeField] private GameObject _deadBobObject;
    [SerializeField] private GameObject _hitmanObject;
    [SerializeField] private GameObject _hitmanGunObject;


    private AudioSource _backgroundAudio;
    private int _storyBeat = 1;
    private int _bobs;


    private void Awake()
    {
        _backgroundAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // debug
        //_backgroundAudio.Play();

        //StartBossInteraction(1);
       // StartBossAct(3);
    }

    private void OnEnable()
    {

        GameEvents.OnBossStart += StartBossAct;
        GameEvents.OnDialogueEnded += EndBossInteraction;
        GameEvents.OnStoryActionPerformed += NextStoryBeat;
        
    }



    private void OnDisable()
    {
        GameEvents.OnBossStart -= StartBossAct;
        GameEvents.OnDialogueEnded -= EndBossInteraction;
        GameEvents.OnStoryActionPerformed -= NextStoryBeat;
    }

    private void StartBossAct(int actNumber)
    {
        //actNumber = 3;
        //PlayerChoices.Instance.SetPlayerFlag("OnClonesStory");

        StartCoroutine(FadeInCoroutine(actNumber));

    }

    private IEnumerator FadeInCoroutine(int actNumber)
    {
        if (actNumber == 3)
        {
            

            if (PlayerChoices.Instance.OnHitmanStory)
            {
                _chairObject.SetActive(false);
                _deadBobObject.SetActive(true);
            }
        }

        _animator.SetTrigger("EnterOffice");
        yield return new WaitForSecondsRealtime(2f);

        StartBossInteraction(actNumber);
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
                    //if (PlayerChoices.Instance.PlantHealth == 2)
                    //{
                    //    thisInteraction = _hitmanBobLivesFinale;
                    //}
                    //else
                    //{
                    thisInteraction = _hitmanBobDiesFinale;
                    //}
                }
                else
                {
                    thisInteraction = _clonesFinale;
                }
                break;
        }

        DialogueHandler.Instance.StartDialogue(thisInteraction.Dialogues, _bossVoiceClip, INTERACTIONOBJECT_ID);

    }


    private void NextStoryBeat()
    {
        if (PlayerChoices.Instance.OnLizardmanStory)
        {
            _chairObject.SetActive(false);
            _lizardBossObject.SetActive(true);
        }
        else if (PlayerChoices.Instance.OnHitmanStory)
        {
            if (_storyBeat == 1)
            {
                _hitmanObject.SetActive(true);
            }
            else if (_storyBeat == 2)
            {
                _hitmanGunObject.SetActive(true);
                _hitmanObject.SetActive(false);
            }
        }
        else
        {
            _chairObject.SetActive(false);

            _bobsObjects[_bobs].SetActive(true);
            _bobs++;
        }

        _storyBeat++;
        
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
