using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarrotGrowth : MonoBehaviour
{
    // Plant growth phases
    public GameObject plantStem;
    public GameObject halfPlant;
    public GameObject fullPlant;

    // Progress circle
    public Canvas progressCanvas;
    public Image progressCircle;

    // Player's shovel and plant animation trigger
    [SerializeField] private GameObject shovel;
    [SerializeField] private GameObject watering_can;
    public Animator playerAnimator;

    private int growingPhase = 0;
    private bool growing = false;

    private bool isFarmingMode = true;

    private void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator = FindObjectOfType<Animator>(); // Ensure player animator is found
        }

        if (shovel == null)
        {
            shovel = GameObject.Find("player/character-male-b/root/torso/arm-left/shovel");
        }
        if (watering_can == null)
        {
            watering_can = GameObject.Find("player/character-male-b/root/torso/arm-left/watering_can");
        }
    }

    private void Start()
    {
        // All phases start as inactive
        plantStem.SetActive(false);
        halfPlant.SetActive(false);
        fullPlant.SetActive(false);
        progressCanvas.gameObject.SetActive(false);

        // Progress circle is not visible
        if (progressCircle != null)
            progressCircle.fillAmount = 0f;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, -3.27f, currentPosition.z);

        // checks if they player is in farming mode
        if (Input.GetKeyDown(KeyCode.E))
        {
            isFarmingMode = false;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFarmingMode = true;
        }
    }

    // Player interacts with plot
    private void OnMouseDown()
    {
        // Checks if planting has started and growth is not in progress
        if (isFarmingMode && !growing)
        {
            // Begins progress circle
            progressCanvas.gameObject.SetActive(true);
            // Starting growing process
            StartCoroutine(HandleGrowth());
        }
    }

    private IEnumerator HandleGrowth()
    {
        growing = true;

        if (growingPhase == 0)   // Planting Phase
        {
            // Planting shovel animation
            if (playerAnimator != null)
                playerAnimator.SetBool("isPlanting", true);

            if (shovel != null)
                shovel.SetActive(true);

            // Timer for 3 seconds for planting animation
            yield return StartCoroutine(FillBar(0.25f, 3f));

            plantStem.SetActive(true);  // Stem asset appears
            growingPhase++;  // Move to next phase

            // Hide the shovel after planting is done
            if (shovel != null)
                shovel.SetActive(false);

            // Reset the attack animation and unlock movement
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isPlanting", false);  // Reset attack animation trigger
            }
        }
        else if (growingPhase == 1)  // Watering Phase
        {
            // Watering animation
            if (playerAnimator != null)
                playerAnimator.SetBool("isWatering", true);

            if (watering_can != null)
                watering_can.SetActive(true);

            // Timer for 5 seconds
            yield return StartCoroutine(FillBar(0.5f, 5f));
            plantStem.SetActive(false);
            halfPlant.SetActive(true);    // Half plant asset appears

            if (watering_can != null)
                watering_can.SetActive(false);

            // Reset animation
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isWatering", false);
            }

            StartCoroutine(GrowthPhase());  // Growing starts without user interaction
            // Wait for growth phase
            yield return new WaitUntil(() => !growing);
            growingPhase++;    // Move to next phase
        }
        else if (growingPhase == 2)  // Harvesting Phase
        {
            // Timer for 3 seconds
            yield return StartCoroutine(FillBar(0f, 3f));
            fullPlant.SetActive(false);
            progressCanvas.gameObject.SetActive(false); // Hide progress circle
            growingPhase = 0;   // Reset phase
            ResetPlot();        // Reset plot
        }

        growing = false;
    }

    private IEnumerator GrowthPhase()
    {
        growing = true;
        // Timer for 4 seconds
        yield return StartCoroutine(FillBar(1f, 4f));
        halfPlant.SetActive(false);
        fullPlant.SetActive(true);    // Full plant asset appears
        growing = false;    // Growing is done
    }

    // Fill tehe progress bar to a given percent and time
    private IEnumerator FillBar(float fillPercent, float duration)
    {
        float startFill = progressCircle.fillAmount;
        float timePassed = 0f;

        // Fills the progress bar during the given time
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            progressCircle.fillAmount = Mathf.Lerp(startFill, fillPercent, timePassed / duration);
            yield return null;
        }

        progressCircle.fillAmount = fillPercent;
    }

    // Reset the plot after harvestingd
    private void ResetPlot()
    {
        plantStem.SetActive(false);
        halfPlant.SetActive(false);
        fullPlant.SetActive(false);
        progressCanvas.gameObject.SetActive(false);
        progressCircle.fillAmount = 0f;
    }
}