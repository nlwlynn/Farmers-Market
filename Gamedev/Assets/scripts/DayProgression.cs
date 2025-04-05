using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayProgression : MonoBehaviour
{
    public int currentDay = 1;
    public int currentCoinGoal = 15; 
    private int moneyEarned = 0;

    public void StartNewDay()
    {
        currentDay++;
        currentCoinGoal = Mathf.Max(50, Mathf.RoundToInt(moneyEarned * 1.1f));
        Debug.Log($"Day {currentDay} started. Coin goal: {currentCoinGoal}");
    }

    public void EndDay(int coinsEarnedToday)
    {
        moneyEarned = coinsEarnedToday;
        Debug.Log($"Day {currentDay} ended. Coins earned: {coinsEarnedToday}");
    }
}
