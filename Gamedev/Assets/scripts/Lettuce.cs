using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lettuce : MonoBehaviour
{
    public GameObject lettuce;

    void Update()
    {
        // checks if the player clicks on the npc and is holding the vegtable
        if (FarmManager.IsHolding && Input.GetMouseButtonDown(0))
        {
            // creates a ray from the mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // checks if the NPC is tagged
                if (hit.collider != null && hit.collider.CompareTag("NPC"))
                {
                    // gives the vegtable to npc
                    lettuce.SetActive(false);
                    FarmManager.IsHolding = false;
                }
            }
        }
    }

    // player is holding vegtable
    public void StartHoldingLettuce()
    {
        FarmManager.IsHolding = true;
        lettuce.SetActive(true);
    }
}
