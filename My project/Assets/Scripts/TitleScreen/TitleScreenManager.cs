using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void StartGame()
    {
        StartCoroutine(LoadLevelAnimation());
        //animator.SetTrigger("Start");
        //SceneManager.LoadScene(1);
    }


    IEnumerator LoadLevelAnimation()
    {
        yield return StartCoroutine(FadeToBlack());
        
        //GameManager.Instance.StartGame();
    }

    private IEnumerator FadeToBlack()
    {
        animator.SetTrigger("Start");
        
        //SceneManager.LoadScene(1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void Credits()
    {
        SceneManager.LoadScene(4);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
