using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    public GameObject pumpkin;
    private bool isHolding = false;

    void Update()
    {
        // checks if the player clicks on the npc and is holding the vegtable
        if (isHolding && Input.GetMouseButtonDown(0))
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
                    pumpkin.SetActive(false);
                    isHolding = false;
                }
            }
        }
    }

    // player is holding vegtable
    public void StartHoldingPumpkin()
    {
        isHolding = true;
        pumpkin.SetActive(true);
    }
}
