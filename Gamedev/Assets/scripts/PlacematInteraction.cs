using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacematInteraction : MonoBehaviour
{
    public string currentItem = "";
    public UIController uiController;

    public GameObject carrot1;
    public GameObject broccoli1;

    [SerializeField] private GameObject carrot;
    [SerializeField] private GameObject broccoli;

    public Carrot carrotScript;
    public Broccoli broccoliScript;

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
        if (broccoli == null)
        {
            broccoli = GameObject.Find("player/character-male-b/root/torso/arm-left/broccoli");
        }
        if (broccoli != null)
        {
            broccoliScript = broccoli.GetComponent<Broccoli>();
        }
    }

    public void PlaceInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot1.SetActive(true);
            currentItem = "carrot";
        }
        else if (item.ToLower() == "broccoli")
        {
            broccoli1.SetActive(true);
            currentItem = "broccoli";
        }
    }

    public void TakeInteract(string item)
    {
        if (item.ToLower() == "broccoli")
        {
            broccoli1.SetActive(false);
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
                    else if (currentItem == "broccoli")
                    {
                        TakeInteract("broccoli");

                        if (carrotScript != null)
                        {
                            broccoliScript.StartHoldingBroccoli();
                        }
                    }

                }
            }
        }
    }
}
