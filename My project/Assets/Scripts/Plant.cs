using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Sprite _healthySprite;
    [SerializeField] private Sprite _dyingSprite;
    [SerializeField] private Sprite _deadSprite;

    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    private bool _isDead;
    private bool _isTutorial = true;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlantWatered += PlantWatered;
    }

    private void OnDisable()
    {
        GameEvents.OnPlantWatered -= PlantWatered;
    }

    public void InitPlant()
    {
        var plantStatus = PlayerChoices.Instance.PlantHealth;

        if (PlayerChoices.Instance.PlantDied)
        {
            _renderer.sprite = _deadSprite;
            _isDead = true;
        }
        else if (PlayerChoices.Instance.PlantIsDying)
        {
            _renderer.sprite = _dyingSprite;
        }
        else
        {
            _renderer.sprite = _healthySprite;
        }

    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueHandler.Instance.IsDialogueOpen)
            return;


        _collider.enabled = false;
        Debug.Log("Watering plant!");

        _audioSource.Play();

        GameEvents.WaterPlant();
        
        if (_isTutorial)
        {
            GameEvents.NextTutorialStep();
            _isTutorial = false;
        }

    }


    public void UpdatePlantStatus(bool isDying)
    {
        if (_isDead)
            return;

        if (isDying)
        {
            // set sprite to dying
            Debug.Log("Plant is dying!");
            _renderer.sprite = _dyingSprite;
            PlayerChoices.Instance.SetPlayerFlag("PlantIsDying");

            if (PlayerChoices.Instance.PlantHealth == 2)
            {
                PlayerChoices.Instance.ChangePlayerValue("PlantHealth", -1);
            }            
        }
        else
        {
            _renderer.sprite = _healthySprite;
        }       
    }

    private void PlantWatered()
    {
        _renderer.sprite = _healthySprite;
        PlayerChoices.Instance.UnsetPlayerFlag("PlantIsDying");
        if (PlayerChoices.Instance.PlantHealth == 1)
        {
            PlayerChoices.Instance.ChangePlayerValue("PlantHealth", 1);
        }        
    }


    public void PlantDies()
    {
        _isDead = true;
        _renderer.sprite = _deadSprite;
        PlayerChoices.Instance.SetPlayerFlag("PlantDied");
        PlayerChoices.Instance.ChangePlayerValue("PlantHealth", -1);
    }

    public void DoTutorial()
    {
        _isTutorial = true;
        _collider.enabled = true;
    }

}
