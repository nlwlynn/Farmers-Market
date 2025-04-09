using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    public GameObject pumpkin;
    private NPCInteraction npcInteraction;
    public UIController uiController;
    private PlacematInteraction placematInteraction;
    private BowlScript bowlScript;
    private PumpkinGrowth pumpkinGrowth;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        pumpkinGrowth = FindObjectOfType<PumpkinGrowth>();
    }

    void Start()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
        placematInteraction = FindObjectOfType<PlacematInteraction>();
        bowlScript = FindObjectOfType<BowlScript>();
    }

    void Update()
    {
        if (uiController.IsNightPhase)
        {
            // hide the veggie
            pumpkin.SetActive(false);
            FarmManager.IsHolding = false;
            FarmManager.IsAnimationPlaying = false;
        }
        // checks if the player clicks on the npc and is holding the vegtable
        if (FarmManager.IsHolding && Input.GetMouseButtonDown(0))
        {
            // creates a ray from the mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    // checks if the NPC is tagged
                    if (hit.collider.CompareTag("NPC1") || hit.collider.CompareTag("NPC2") || hit.collider.CompareTag("NPC3") || hit.collider.CompareTag("NPC4"))
                    {
                        NPCInteraction npcInteraction = hit.collider.GetComponent<NPCInteraction>();

                        if (npcInteraction != null)
                        {
                            // give the veggie to the NPC
                            if(npcInteraction.Interact("Corn"))
                            {
                                // hide the veggie
                                pumpkin.SetActive(false);
                                FarmManager.IsHolding = false;
                                FarmManager.IsAnimationPlaying = false;
                                pumpkinGrowth.SetFarmingMode(true);
                            }
                        }
                    }
                    else if (hit.collider.CompareTag("Placemat1") || hit.collider.CompareTag("Placemat2"))
                    {
                        PlacematInteraction placematInteraction = hit.collider.GetComponent<PlacematInteraction>();

                        if (placematInteraction != null)
                        {
                            placematInteraction.PlaceInteract("pumpkin");
                            pumpkin.SetActive(false);
                            FarmManager.IsHolding = false;
                            FarmManager.IsAnimationPlaying = false;
                            pumpkinGrowth.SetFarmingMode(true);
                        }
                    }
                    else if (hit.collider.CompareTag("Bowl"))
                    {
                        BowlScript bowlScript = hit.collider.GetComponent<BowlScript>();

                        if (bowlScript != null)
                        {
                            bowlScript.BowlInteract("pumpkin");
                            pumpkin.SetActive(false);
                            FarmManager.IsHolding = false;
                            FarmManager.IsAnimationPlaying = false;
                            pumpkinGrowth.SetFarmingMode(true);
                        }
                    }
                }
            }
        }
    }

    // player is holding vegtable
    public void StartHoldingPumpkin()
    {
        FarmManager.IsHolding = true;
        pumpkin.SetActive(true);
    }
}