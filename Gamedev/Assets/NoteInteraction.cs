using UnityEngine;

public class NoteInteraction : MonoBehaviour {
    public GameObject notePanel; // Assign in Inspector
    public LayerMask interactableLayer; // Assign in Inspector

    void Update()
    {
        // Early exit if notePanel is not assigned
        if (notePanel == null)
        {
            Debug.LogError("notePanel is not assigned in the Inspector!");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Debug: Draw the ray in Scene view (visible in Play Mode)
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == gameObject) // Only open if clicking THIS note
                {
                    notePanel.SetActive(true);
                    Debug.Log("Note opened!");
                }
            }
            else if (notePanel.activeSelf) // Clicked outside the note
            {
                notePanel.SetActive(false);
                Debug.Log("Note closed (clicked outside).");
            }
        }
    }
}