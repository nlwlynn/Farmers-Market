using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;
    public GameObject shopPanel; // Reference to the Shop Canvas
    // Start is called before the first frame update
    public UIController uiController; // Reference to UIController
    public Inventory inventory; // Reference to the Inventory Manager
    public bool closeBuild = false;
    public PlacementSystem placementSystem;

    void Awake()
    {
        if (placementSystem == null)
        {
            placementSystem = FindObjectOfType<PlacementSystem>();
        }
    }
    void Start()
    {
       
        for (int i = 0; i < shopItemsSO.Length; i++)
            shopPanelsGO[i].SetActive(true);

        LoadPanels();
        CheckPurchaseable();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shopPanel.activeSelf && closeBuild)
        {
            closeBuild = false;
        }
    }

    // Update is called once per frame
    public void AddCoins()
    {
        UIController.Instance.AddCoins(1);
        //Debug.Log("Coins Added!");
        CheckPurchaseable();
    }

    public void TestButton()
    {
        Debug.Log("Button Clicked!");
    }


    public void CheckPurchaseable()
    {
        int currentCoins = UIController.Instance.GetCoins(); // Get global coins

        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            myPurchaseBtns[i].interactable = (currentCoins >= shopItemsSO[i].baseCost);
        }
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].titleTxt.text = shopItemsSO[i].title;
            shopPanels[i].descriptionTxt.text = shopItemsSO[i].description;
            shopPanels[i].costTxt.text = "Cost: " + shopItemsSO[i].baseCost.ToString()+ " Coins";
            shopPanels[i].profitTxt.text = "Profit: " + shopItemsSO[i].profit.ToString()+" Coins";
            if (shopItemsSO[i].itemImage != null) // Ensure there's an image
            {

                //Debug.Log("Setting sprite for: " + shopItemsSO[i].title);
                shopPanels[i].itemImage.sprite = shopItemsSO[i].itemImage;
            }
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (UIController.Instance.GetCoins() >= shopItemsSO[btnNo].baseCost)
        {
            UIController.Instance.SpendCoins(shopItemsSO[btnNo].baseCost); // Deduct coins globally
            //Debug.Log($"Purchased Item: {shopItemsSO[btnNo].title} (Index: {btnNo})");

            inventory.AddItemToStock(btnNo);
            CheckPurchaseable();
        }
        else
        {
            Debug.Log("Not enough coins to purchase this item!");
        }
    }

    public void Exit()
    {
        shopPanel.SetActive(false);
        closeBuild = true;
        placementSystem.stopAll();

        if (GetComponent<InputManager>() != null)
        {
            GetComponent<InputManager>().SwitchCameraOff();
        }
        else
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            if (inputManager != null)
            {
                inputManager.SwitchCameraOff();
            }
        }
    }
}


