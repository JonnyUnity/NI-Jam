using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ViewManager _viewManager;
    [SerializeField] private DialogueHandler _dialogueHandler;

    [SerializeField] private GameObject _exampleObject;
    [SerializeField] private GameObject _walkingPersonObject;

    [SerializeField] private GameObject _bobTestObject;

    private PlayerChoices _playerChoices;
    private int _actActions;

    private void Awake()
    {

        //_playerChoices = new PlayerChoices();

        SceneManager.sceneLoaded += SceneLoaded;
    
        //GameEvents.OnWaterPlant += WateredPlant;



        DontDestroyOnLoad(gameObject);
    }

    

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // set num possible actions
        _actActions = 10;

    }

    //private void WateredPlant()
    //{
    //    int wateredCount = _playerChoices.WateredPlant();
    //    _actActions--;
    //    _viewManager.UpdateDebug(wateredCount.ToString());
    //}

    private void OnDisable()
    {
        //GameEvents.OnWaterPlant -= WateredPlant;
    }


    // Start is called before the first frame update
    void Start()
    {
        ActivateWalkingPerson();

        Invoke("BobVisit1", 6f);

    }

    // Update is called once per frame
    void Update()
    {

    }


    //public void WaterPlant()
    //{
    //    GameEvents.WaterPlant();
    //}

    public void ActivateTelephone()
    {
        _exampleObject.SetActive(true);
        GameEvents.ActivateObject(1);
    }


    public void ActivateWalkingPerson()
    {
        _walkingPersonObject.SetActive(true);
    }


    private void BobVisit1()
    {
        _bobTestObject.SetActive(true);

        var bobInteractionContainer = _bobTestObject.GetComponent<InteractionContainer>();
        var interaction = bobInteractionContainer.GetInteraction(1);

        DialogueHandler.Instance.StartDialogue(interaction.Dialogues, interaction.Responses);


    }

}
