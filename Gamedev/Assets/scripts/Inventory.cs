using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class Inventory : MonoBehaviour
{
    public int stock = 0;
    public GameObject inventoryPanel; // Reference to the Shop Canvas
    // Start is called before the first frame update
    public UIController uiController; // Reference to UIController

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Exit()
    {
        inventoryPanel.SetActive(false);
        Debug.Log("Inventory closed!");
    }
}


