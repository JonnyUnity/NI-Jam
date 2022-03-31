using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Watercooler : MonoBehaviour
{

    [SerializeField] private GameObject _highlight;
    [SerializeField] private AudioClip _useCoolerClip;

    private bool _isTutorial;

    private SpriteRenderer _renderer;
    private AudioSource _audioSource;
    private Animator _animator;
    private BoxCollider2D _collider;

    private bool StopInteracting => (EventSystem.current.IsPointerOverGameObject() || DialogueHandler.Instance.IsDialogueOpen);

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _useCoolerClip;
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }

    public void EnableWaterCooler()
    {
        _collider.enabled = true;
    }

    public void EnableTutorial()
    {
        _isTutorial = true;
    }


    private void OnMouseOver()
    {
        if (StopInteracting)
            return;

        _highlight.SetActive(true);

    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (StopInteracting)
            return;

        Debug.Log("Used water cooler!");
        _animator.SetTrigger("UseWatercooler");
        _audioSource.Play();

        if (_isTutorial)
        {

            _isTutorial = false;
            //GameEvents.ProgressStory();
            GameEvents.NextTutorialStep();
        }

        _collider.enabled = false;
        GameEvents.WaterCollected();

    }


}
