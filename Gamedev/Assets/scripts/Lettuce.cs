using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lettuce : MonoBehaviour
{
    public GameObject lettuce;
    private NPCInteraction npcInteraction;
    public UIController uiController;
    private PlacematInteraction placematInteraction;
    private LettuceGrowth lettuceGrowth;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        lettuceGrowth = FindObjectOfType<LettuceGrowth>();
    }

    void Start()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
        placematInteraction = FindObjectOfType<PlacematInteraction>();
    }

    void Update()
    {
        if (uiController.IsNightPhase)
        {
            // hide the veggie
            lettuce.SetActive(false);
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
                            npcInteraction.Interact("Mushroom");

                            // hide the veggie
                            lettuce.SetActive(false);
                            FarmManager.IsHolding = false;
                            FarmManager.IsAnimationPlaying = false;
                            lettuceGrowth.SetFarmingMode(true);
                        }
                    }
                    else if (hit.collider.CompareTag("Placemat1") || hit.collider.CompareTag("Placemat2"))
                    {
                        PlacematInteraction placematInteraction = hit.collider.GetComponent<PlacematInteraction>();

                        if (placematInteraction != null)
                        {
                            placematInteraction.PlaceInteract("lettuce");
                            lettuce.SetActive(false);
                            FarmManager.IsHolding = false;
                            FarmManager.IsAnimationPlaying = false;
                            lettuceGrowth.SetFarmingMode(true);
                        }
                    }
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