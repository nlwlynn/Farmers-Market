using System.Diagnostics;
using UnityEngine;

public class NoteInteraction : MonoBehaviour {
    public GameObject notePanel;
    public LayerMask interactableLayer; // Assign in Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
            {
                UnityEngine.Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == gameObject) // Only open note if clicking paper
                {
                    UnityEngine.Debug.Log("Show Note");
                    notePanel.SetActive(true);
                }
            }
            else if (notePanel.activeSelf) // Clicked outside
            {
                UnityEngine.Debug.Log("Hide Note");
                notePanel.SetActive(false);
            }
        }
    }
}
