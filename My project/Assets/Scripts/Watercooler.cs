using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watercooler : MonoBehaviour
{

    private bool _isTutorial = true;

    private void OnMouseDown()
    {
        Debug.Log("Used water cooler!");

        if (_isTutorial)
        {

            _isTutorial = false;
            GameEvents.ProgressStory();
        }

    }


}
