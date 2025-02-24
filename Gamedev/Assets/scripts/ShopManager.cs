using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ShopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinUI;
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;
    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < shopItemsSO.Length;i++)
        //    shopPanelsGO[i].SetActive(true);
        coinUI.text = "Coins: " + coins.ToString();
        //LoadPanels();
        //CheckPurchaseable();
        
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
        //CheckPurchaseable(); 
    }
}
