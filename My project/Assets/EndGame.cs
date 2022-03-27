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

            _endingText.text = "You reached the lizardman ending";
            if (PlayerChoices.Instance.Lizardman_Survived)
            {
                _twistText.text = "and was not eaten";
            }
            else
            {
                _twistText.text = "But you were eaten";
            }

        }
        else if (PlayerChoices.Instance.OnHitmanStory)
        {
            _audioSource.clip = _hitmanEndingClip;


            _endingText.text = "You reached the hitman ending";
            if (PlayerChoices.Instance.Hitman_Survived)
            {
                _twistText.text = "and you lived thanks to Bob's sacrifice";
            }
            else
            {
                _twistText.text = "and Bob lived thanks to your sacrifice";
            }

        }
        else // Clones story
        {
            _audioSource.clip = _clonesEndingClip;

            _endingText.text = "You reached the clones ending";
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
        SceneManager.LoadScene(0);
    }

}
