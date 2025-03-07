using UnityEngine;

public class Broccoli : MonoBehaviour
{
    public GameObject broccoli;
    private NPCInteraction npcInteraction;

    void Start()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
    }

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
                    broccoli.SetActive(false);
                    FarmManager.IsHolding = false;

                    // Updates NPC
                    if (npcInteraction != null)
                    {
                        npcInteraction.Interact("Broccoli");
                    }
                }
            }
        }
    }

    // player is holding vegtable
    public void StartHoldingBroccoli()
    {
        FarmManager.IsHolding = true;
        broccoli.SetActive(true);
    }
}