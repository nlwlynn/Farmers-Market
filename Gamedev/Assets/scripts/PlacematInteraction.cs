using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacematInteraction : MonoBehaviour
{
    public GameObject carrot1;

    public void PlaceInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot1.SetActive(true);
        }
    }

    public void TakeInteract()
    {

    }
}
