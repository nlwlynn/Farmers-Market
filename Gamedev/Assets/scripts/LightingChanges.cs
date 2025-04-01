using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LightingChanges : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;  
    public PostProcessProfile dayProfile;  
    public PostProcessProfile nightProfile; 
    public UIController uiController;  

    private void Start()
    {
        UpdatePostProcessProfile();
    }

    void Update()
    {
        // Switch lighting between day/night phase
        if (uiController.isNightPhase)
        {
            SetNightProfile();
        }
        else
        {
            SetDayProfile();
        }
    }

    void SetDayProfile()
    {
        if (postProcessVolume.profile != dayProfile)
        {
            postProcessVolume.profile = dayProfile;
        }
    }

    void SetNightProfile()
    {
        if (postProcessVolume.profile != nightProfile)
        {
            postProcessVolume.profile = nightProfile;
        }
    }

    void UpdatePostProcessProfile()
    {
        // Checks if night/day
        if (uiController.isNightPhase)
        {
            postProcessVolume.profile = nightProfile;
        }
        else
        {
            Debug.Log("WHYYY CRUEL WORLD");
            postProcessVolume.profile = dayProfile;
        }
    }
}
