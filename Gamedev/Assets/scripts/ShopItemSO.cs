using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shopMenu", menuName = "scriptable objects/New shop Item", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public string title;
    public int ID;
    public string description;
    public int baseCost;
    public int profit;
    public Sprite itemImage;

    public bool isOneTimePurchase = false;
    [HideInInspector] public bool hasBeenPurchased = false;

}
