using UnityEngine;

public class Broccoli : MonoBehaviour
{
    public GameObject broccoli;  // Reference to the broccoli object
    private bool isHoldingBroccoli = false;  // Track if the player is holding the broccoli

    void Update()
    {
        // If the player clicks and is holding the broccoli
        if (isHoldingBroccoli && Input.GetMouseButtonDown(0))  // 0 for left mouse button
        {
            Debug.Log("Player clicked the mouse while holding broccoli.");

            // Create a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is tagged as "NPC"
                if (hit.collider != null && hit.collider.CompareTag("NPC"))
                {
                    // Give the broccoli to the NPC
                    broccoli.SetActive(false);  // Make the broccoli disappear when given
                    isHoldingBroccoli = false;  // Player is no longer holding the broccoli
                    Debug.Log("Broccoli given to " + hit.collider.gameObject.name);  // Log NPC name
                }
                else
                {
                    Debug.Log("Clicked object is not an NPC: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
    }

    // This function will be used to start holding broccoli (could be triggered by a button or event)
    public void StartHoldingBroccoli()
    {
        isHoldingBroccoli = true;
        broccoli.SetActive(true);  // Make sure broccoli is visible when the player is holding it
        Debug.Log("Player is holding the broccoli.");
    }
}
