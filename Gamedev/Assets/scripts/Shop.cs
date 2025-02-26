using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int Coin;
    public int Carrot;
    public Text Coin_text;
    public Text Carrot_text; 


    // Start is called before the first frame update
    void Start()
    {
        Coin = 100;
        Coin_text.text = Coin.ToString();
    }

    public void BuyCarrot()
    {
        if (Coin >= 20)
        {
            Coin -= 20;
            Coin_text.text = Coin.ToString();

            Carrot += 10;
            Carrot_text.text= Carrot.ToString();
        }
        else 
        {
            print("Not enough Coins");

        }
    }

}
