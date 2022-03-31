using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject _pauseMenu;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.SetActive(!_pauseMenu.activeInHierarchy);
        }
    }


    public void ContinueGame()
    {
        _pauseMenu.SetActive(false);
    }


    public void QuitToTitle()
    {
        _pauseMenu.SetActive(false);
        Destroy(GameManager.Instance.gameObject);
        Destroy(DialogueHandler.Instance.gameObject);
        SceneManager.LoadScene(0);
    }


}
