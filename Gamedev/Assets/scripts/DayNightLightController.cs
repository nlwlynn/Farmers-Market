using UnityEngine;

public class DayNightLightController : MonoBehaviour {
    public Light dayLight;
    public Light nightLight;

    void Update() {
        // Make sure the UIController.Instance exists before using it
        if (UIController.Instance != null) {
            bool isNight = UIController.Instance.isNightPhase;

            // Toggle light based on night phase
            dayLight.enabled = !isNight;
            nightLight.enabled = isNight;
        }
        else {
            // Optional: turn both off or log a warning during early frames
            dayLight.enabled = true;
            nightLight.enabled = false;
        }
    }
}