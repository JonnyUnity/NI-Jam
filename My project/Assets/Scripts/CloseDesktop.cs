using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDesktop : MonoBehaviour
{

    private void OnMouseDown()
    {
        GameEvents.CloseDesktop();
    }

}
