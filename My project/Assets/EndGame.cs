using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{

    [SerializeField] private TMP_Text _endingText;
    [SerializeField] private TMP_Text _twistText;
    [SerializeField] private AudioClip _lizardmanEndingClip;
    [SerializeField] private AudioClip _hitmanEndingClip;
    [SerializeField] private AudioClip _clonesEndingClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        

        if (PlayerChoices.Instance.OnLizardmanStory)
        {
            _audioSource.clip = _lizardmanEndingClip;

            _endingText.text = "After not taking the company mandated pills you saw the true reality of lizard people living amongst us";
            if (PlayerChoices.Instance.Lizardman_Survived)
            {
                _twistText.text = "and was not eaten";
            }
            else
            {
                _twistText.text = "and you were eaten";
            }

        }
        else if (PlayerChoices.Instance.OnHitmanStory)
        {
            _audioSource.clip = _hitmanEndingClip;


            _endingText.text = "The company sent a hitman to take you and Bob out after you investigated their finances";
            if (PlayerChoices.Instance.Bob_survives_Hitman)
            {
                _twistText.text = "and Bob lived thanks to your sacrifice";
            }
            else
            {
                _twistText.text = "and Bob was killed by the hitman";
            }

        }
        else // Clones story
        {
            _audioSource.clip = _clonesEndingClip;

            _endingText.text = "You discovered Bob's master plan to assimilate the population into an army of bobs";
            if (PlayerChoices.Instance.Clones_Survived)
            {
                _twistText.text = "and got to continue living your life";
            }
            else
            {
                _twistText.text = "and were swapped to a Bob clone";
            }

        }

        _audioSource.Play();
    }


    public void BackToStart()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(DialogueHandler.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

}
