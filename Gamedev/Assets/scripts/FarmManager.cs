using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static bool IsAnimationPlaying { get; set; }
    public static bool IsHolding { get; set; }

    private void Awake()
    {
        // Initialize IsHolding to false at the start of the game
        IsHolding = false;
    }
}

