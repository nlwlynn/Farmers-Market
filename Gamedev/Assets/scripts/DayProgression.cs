using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayProgression : MonoBehaviour
{
    public int currentDay = 0;
    public int currentCoinGoal = 15; 
    private int moneyEarned = 0;
    public FlyAI flyAI;

    private void Awake()
    {
        flyAI = FindObjectOfType<FlyAI>();
    }

    public void EndDayEarnings(int coinsEarnedToday, int getDay)
    {
        moneyEarned = coinsEarnedToday;
        currentDay = getDay;
        StartNewDay();
    }

    public void StartNewDay()
    {
        flyAI.IncreaseFlyDifficulty();

        if (currentDay == 1)
        {
            currentCoinGoal = 15;
        }
        else
        {
            int calculatedGoal = Mathf.RoundToInt(moneyEarned * 1.1f);
            int minIncrease = currentCoinGoal + 5; 
            currentCoinGoal = Mathf.Max(calculatedGoal, minIncrease, 15); 
        }
    }

    public int NewGoal()
    {
        return currentCoinGoal;
    }

    public void ResetProgression()
    {
        currentDay = 0;
        currentCoinGoal = 15;
        moneyEarned = 0;
}
}
