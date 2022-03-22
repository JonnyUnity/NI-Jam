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
        SceneManager.LoadScene(1);
    }

    private IEnumerator FadeToBlack()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1);

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
