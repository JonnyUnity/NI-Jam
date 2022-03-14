using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionContainer : MonoBehaviour
{

    [SerializeField] private List<Interaction> _interactions;


    public Interaction GetInteraction(int interactionID)
    {
        return _interactions.Where(w => w.ID == interactionID).SingleOrDefault();
    }


}
