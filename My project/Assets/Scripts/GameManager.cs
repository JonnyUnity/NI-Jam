using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] private Animator _animator;
    //[SerializeField] private TMPro.TMP_Text _dayHeadingText; 

    private PlayerChoices _playerChoices;

    private int _actNumber = 1;
    public PlayerState State;

    private void Awake()
    {

        //_playerChoices = new PlayerChoices();

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEvents.OnActOver += LoadBossOfficeScene;
        GameEvents.OnBossEnd += LoadNextScene;

        DontDestroyOnLoad(Instance.gameObject);

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.buildIndex == 2)
        {
            StartCoroutine(LoadDeskCoroutine());
        }
        else if (scene.buildIndex == 3)
        {
            StartCoroutine(LoadBossOfficeSceneCoroutine());
        }

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEvents.OnActOver -= LoadBossOfficeScene;
        GameEvents.OnBossEnd -= LoadNextScene;

    }


    void Start()
    {
        //ActivateWalkingPerson();
        //_actNumber++;
        //GameEvents.StartAct(_actNumber);

        //Invoke("BobVisit1", 6f);
        // Invoke(nameof(CallPhone), 4f);

        //SceneManager.LoadScene(2);
        StartGame();
        
        // Debug boss scene
        //SceneManager.LoadScene(3);
    }


    public void UpdateStateToIdle()
    {
        State = PlayerState.IDLE;
    }

    public void UpdateStateToBusy()
    {
        State = PlayerState.BUSY;
    }



    public void StartGame()
    {

        PlayerChoices.Instance.Init();
        _actNumber = 1;
        SceneManager.LoadScene(2);

    }

    private void LoadNextScene()
    {
        if (_actNumber == 4)
        {
            // game over
            SceneManager.LoadScene(5);
        }
        else
        {
            SceneManager.LoadScene(2);  // desk
        }

    }


    private IEnumerator LoadDeskCoroutine()
    {
        yield return new WaitForSeconds(1);

        GameEvents.StartAct(_actNumber);

    }


    private void LoadBossOfficeScene()
    {
        SceneManager.LoadScene(3);

        //GameEvents.StartBoss(_actNumber);
        //_actNumber++;

        //StartCoroutine(LoadBossOfficeSceneCoroutine());
    }


    private IEnumerator LoadBossOfficeSceneCoroutine()
    {

        //_animator.SetTrigger("EndOfDay");
        //yield return new WaitForSeconds(1);

        //SceneManager.LoadScene(3);

        yield return new WaitForSeconds(1);

        GameEvents.StartBoss(_actNumber);
        _actNumber++;

    }



}
