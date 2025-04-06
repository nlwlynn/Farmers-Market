using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacematInteraction : MonoBehaviour
{
    public GameObject carrot1;
    public string currentItem = "";
    public UIController uiController;
    [SerializeField] private GameObject carrot;
    public Carrot carrotScript;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        if (carrot == null)
        {
            carrot = GameObject.Find("player/character-male-b/root/torso/arm-left/carrot");
        }
        if (carrot != null)
        {
            carrotScript = carrot.GetComponent<Carrot>();
        }
    }

    public void PlaceInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot1.SetActive(true);
            currentItem = "carrot";
        }
    }

    public void TakeInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot1.SetActive(false);
            currentItem = "";
        }
    }

    void Update()
    {
        if (!FarmManager.IsHolding && Input.GetMouseButtonDown(0) && !uiController.IsNightPhase)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if this placemat was clicked
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (currentItem == "carrot")
                    {
                        TakeInteract("carrot");

                        if (carrotScript != null)
                        {
                            carrotScript.StartHoldingCarrot();
                        }
                    }
                }
            }
        }
    }
}
