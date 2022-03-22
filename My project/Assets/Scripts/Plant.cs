using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour
{

    [SerializeField] private Sprite _healthySprite;
    [SerializeField] private Sprite _dyingSprite;

    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    private bool _isTutorial = true;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        GameEvents.OnWaterCollected += ReadyPlant;
    }

    private void OnDisable()
    {
        GameEvents.OnWaterCollected -= ReadyPlant;
    }

    private void ReadyPlant()
    {
        _collider.enabled = true;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;

        Debug.Log("Watering plant!");

        // playwater sound
        
        GameEvents.PlantWatered();

        _collider.enabled = false;
        
        // Plant is healthy again!
        //_renderer.sprite = _healthySprite;

        if (_isTutorial)
        {
            GameEvents.ProgressStory();
            _isTutorial = false;
        }

    }


    public void UpdatePlantStatus(bool isDying)
    {
        
        if (isDying)
        {
            // set sprite to dying
            Debug.Log("Plant is dying!");
        }

        _collider.enabled = true;
                
    }

    public void DoTutorial()
    {
        _collider.enabled = true;
        _isTutorial = true;
    }

}
