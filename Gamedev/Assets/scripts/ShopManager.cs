using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    PlayerSoundEffects soundEffects;  // Reference to the PlayerSoundEffects script

    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;
    public GameObject shopPanel; // Reference to the Shop Canvas
    public UIController uiController; // Reference to UIController
    public Inventory inventory; // Reference to the Inventory Manager
    public bool closeBuild = false;
    public PlacementSystem placementSystem;

    void Awake()
    {
        if (soundEffects == null) {
            soundEffects = FindObjectOfType<PlayerSoundEffects>(); // Get SFXs
        }
        if (placementSystem == null)
        {
            placementSystem = FindObjectOfType<PlacementSystem>();
        }
    }
    void Start()
    {
        ResetOneTimePurchases();

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

    void ResetOneTimePurchases()
    {
        foreach (ShopItemSO item in shopItemsSO)
        {
            item.hasBeenPurchased = false;
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
            if (shopItemsSO[i].isOneTimePurchase && shopItemsSO[i].hasBeenPurchased)
            {
                myPurchaseBtns[i].interactable = false;
            }
            else
            {
                myPurchaseBtns[i].interactable = (currentCoins >= shopItemsSO[i].baseCost);
            }
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
        ShopItemSO item = shopItemsSO[btnNo];

        if (item.isOneTimePurchase && item.hasBeenPurchased)
        {
            Debug.Log($"{item.title} can only be purchased once!");
            return;
        }

        if (UIController.Instance.GetCoins() >= shopItemsSO[btnNo].baseCost)
        {
            UIController.Instance.SpendCoins(shopItemsSO[btnNo].baseCost); // Deduct coins globally
            Debug.Log($"Purchased Item: {shopItemsSO[btnNo].title} (Index: {btnNo})");

            inventory.AddItemToStock(btnNo);
            soundEffects.PlayPurchaseItemSound();

            if (item.isOneTimePurchase)
            {
                item.hasBeenPurchased = true;
                myPurchaseBtns[btnNo].interactable = false; // Disable the button visually
            }

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


