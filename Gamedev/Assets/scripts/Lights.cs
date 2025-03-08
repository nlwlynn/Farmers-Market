using UnityEngine;

public class Lights : MonoBehaviour
{
    public GameObject lightParent;
    private Light[] lights;
    public UIController uiController;

    private void Awake()
    {
        lights = lightParent.GetComponentsInChildren<Light>();

        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
    }

    private void Update()
    {
        // Turn lights on/off based on night/day phase
        if (uiController.IsNightPhase)
        {
            TurnLightsOn();
        }
        else
        {
            TurnLightsOff();
        }
    }

    // Turn all lights on
    private void TurnLightsOn()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }

    // Turn all lights off
    private void TurnLightsOff()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
