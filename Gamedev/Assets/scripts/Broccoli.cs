using UnityEngine;

public class Broccoli : MonoBehaviour
{
    public GameObject broccoli;
    private NPCInteraction npcInteraction;
    public UIController uiController;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
    }

    void Start()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
    }

    void Update()
    {
        if (uiController.IsNightPhase)
        {
            // hide the veggie
            broccoli.SetActive(false);
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
                            if(npcInteraction.Interact("Broccoli"))
                            {
                                // hide the veggie
                                broccoli.SetActive(false);
                                FarmManager.IsHolding = false;
                                FarmManager.IsAnimationPlaying = false;
                            }
                        }
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