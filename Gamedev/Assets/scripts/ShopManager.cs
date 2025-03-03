using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ShopManager : MonoBehaviour
{
    public int coins=20;
    public TMP_Text coinUI;
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;
    public GameObject shopPanel; // Reference to the Shop Canvas
    // Start is called before the first frame update
    public UIController uiController; // Reference to UIController

    void Start()
    {
       
        for (int i = 0; i < shopItemsSO.Length; i++)
            shopPanelsGO[i].SetActive(true);
        coinUI.text = "Coins: " + coins.ToString();
        LoadPanels();
        CheckPurchaseable();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per frame
    public void AddCoins()
    {
        coins++;
        coinUI.text = "Coins: " + coins.ToString();
        Debug.Log("Button Clicked");
        CheckPurchaseable(); 
    }

    public void TestButton()
    {
        Debug.Log("Button Clicked!");
    }


    public void CheckPurchaseable()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            if(coins>= shopItemsSO[i].baseCost)
                myPurchaseBtns[i].interactable = true;
            else
                myPurchaseBtns[i].interactable = false;
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

                Debug.Log("Setting sprite for: " + shopItemsSO[i].title);
                shopPanels[i].itemImage.sprite = shopItemsSO[i].itemImage;
            }
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= shopItemsSO[btnNo].baseCost)
        {
            coins = coins - shopItemsSO[btnNo].baseCost;
            coinUI.text = "Coins: " + coins.ToString();
            CheckPurchaseable();
        }
    }


    public void Exit()
    {
        shopPanel.SetActive(false);
        Debug.Log("Shop closed!");
    }
}


