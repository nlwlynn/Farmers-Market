using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacematInteraction : MonoBehaviour
{
    public string currentItem = "";
    public UIController uiController;

    public GameObject carrot1;
    public GameObject broccoli1;
    public GameObject cauliflower1;
    public GameObject lettuce1;
    public GameObject watermelon1;
    public GameObject pumpkin1;

    [SerializeField] private GameObject carrot;
    [SerializeField] private GameObject broccoli;
    [SerializeField] private GameObject cauliflower;
    [SerializeField] private GameObject lettuce;
    [SerializeField] private GameObject watermelon;
    [SerializeField] private GameObject pumpkin;

    public Carrot carrotScript;
    public Broccoli broccoliScript;
    public Cauliflower cauliflowerScript;
    public Lettuce lettuceScript;
    public Watermelon watermelonScript;
    public Pumpkin pumpkinScript;

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
        if (cauliflower == null)
        {
            cauliflower = GameObject.Find("player/character-male-b/root/torso/arm-left/cauliflower");
        }
        if (cauliflower != null)
        {
            cauliflowerScript = cauliflower.GetComponent<Cauliflower>();
        }
        if (lettuce == null)
        {
            lettuce = GameObject.Find("player/character-male-b/root/torso/arm-left/mushroom");
        }
        if (lettuce != null)
        {
            lettuceScript = lettuce.GetComponent<Lettuce>();
        }
        if (pumpkin == null)
        {
            pumpkin = GameObject.Find("player/character-male-b/root/torso/arm-left/corn");
        }
        if (pumpkin != null)
        {
            pumpkinScript = pumpkin.GetComponent<Pumpkin>();
        }
        if (watermelon == null)
        {
            watermelon = GameObject.Find("player/character-male-b/root/torso/arm-left/sunflower");
        }
        if (watermelon != null)
        {
            watermelonScript = watermelon.GetComponent<Watermelon>();
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
        else if (item.ToLower() == "cauliflower")
        {
            cauliflower1.SetActive(true);
            currentItem = "cauliflower";
        }
        else if (item.ToLower() == "lettuce")
        {
            lettuce1.SetActive(true);
            currentItem = "lettuce";
        }
        else if (item.ToLower() == "pumpkin")
        {
            pumpkin1.SetActive(true);
            currentItem = "pumpkin";
        }
        else if (item.ToLower() == "watermelon")
        {
            watermelon1.SetActive(true);
            currentItem = "watermelon";
        }
    }

    public void TakeInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot1.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "broccoli")
        {
            broccoli1.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "cauliflower")
        {
            cauliflower1.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "lettuce")
        {
            lettuce1.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "pumpkin")
        {
            pumpkin1.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "watermelon")
        {
            watermelon1.SetActive(false);
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

                        if (broccoliScript != null)
                        {
                            broccoliScript.StartHoldingBroccoli();
                        }
                    }
                    else if (currentItem == "cauliflower")
                    {
                        TakeInteract("cauliflower");

                        if (cauliflowerScript != null)
                        {
                            cauliflowerScript.StartHoldingCauliflower();
                        }
                    }
                    else if (currentItem == "lettuce")
                    {
                        TakeInteract("lettuce");

                        if (lettuceScript != null)
                        {
                            lettuceScript.StartHoldingLettuce();
                        }
                    }
                    else if (currentItem == "pumpkin")
                    {
                        TakeInteract("pumpkin");

                        if (pumpkinScript != null)
                        {
                            pumpkinScript.StartHoldingPumpkin();
                        }
                    }
                    else if (currentItem == "watermelon")
                    {
                        TakeInteract("watermelon");

                        if (watermelonScript != null)
                        {
                            watermelonScript.StartHoldingWatermelon();
                        }
                    }
                }
            }
        }
    }
}
