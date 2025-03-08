using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class Inventory : MonoBehaviour
{
    public int[] stock; // Array to store stock for each item
    public TMP_Text[] stockTexts; // UI text for each item's stock
    public GameObject inventoryPanel;
    [SerializeField]
    private ObjectsDatabaseSO database;

    void Start()
    {
        if (stock == null || stock.Length != database.objectsData.Count)
        {
            stock = new int[database.objectsData.Count]; // Ensure stock array is same size as database
        }

        // Debugging: Verify correct stock assignment
        for (int i = 0; i < stock.Length; i++)
        {
            Debug.Log($"Stock[{i}] = {stock[i]} (Database ID: {database.objectsData[i].ID})");
        }

        UpdateStockUI(); // Ensure UI is updated correctly
    }

    // This function is called when an item is purchased
    public void AddItemToStock(int itemIndex)
    {
        Debug.Log($"Before Purchase - Stock[{itemIndex}]: {stock[itemIndex]}");

        stock[itemIndex]++; // Increase stock count for the purchased item

        Debug.Log($"After Purchase - Stock[{itemIndex}]: {stock[itemIndex]}");

        UpdateStockUI(); // Ensure the UI updates immediately
    }


    public Button[] itemButtons; // UI buttons corresponding to inventory items

    public void UpdateStockUI()
    {
        Debug.Log("Updating Stock UI...");

        for (int i = 0; i < stock.Length; i++)
        {
            if (stockTexts[i] != null)
            {
                stockTexts[i].text = "Stock: " + stock[i].ToString();
                stockTexts[i].ForceMeshUpdate();

                // Disable button if stock is 0
                if (itemButtons[i] != null)
                {
                    itemButtons[i].interactable = stock[i] > 0;
                }
            }
            else
            {
                Debug.LogError("StockTexts[" + i + "] is NULL!");
            }
        }
    }

    public void Exit()
    {
        inventoryPanel.SetActive(false);
    }

}


