using System.Collections;
using System.Diagnostics;
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
    [SerializeField] private GameObject sickle;
    [SerializeField] private GameObject carrot;
    public Animator playerAnimator;

    public int growingPhase = 0;
    private bool growing = false;
    public bool harvestGrowth = false;

    // Fly interactions
    public int plantHealth = 0;
    private bool plantActive = false;
    private float damageInterval = 5f;
    private float lastDamageTime = 0f;

    // interactions
    private GameObject player;
    public float interactionRange = 5f;
    private bool isFarmingMode = true;
    public UIController uiController;
    public Rigidbody rb;

    public Carrot carrotScript;

    public bool NPCFarming = false;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.Find("player");
        }
        if (playerAnimator == null)
        {
            playerAnimator = player.transform.Find("character-male-b")?.GetComponent<Animator>();
        }

        if (rb == null)
        {
            rb = player.GetComponent<Rigidbody>();
        }
        if (shovel == null)
        {
            shovel = GameObject.Find("player/character-male-b/root/torso/arm-left/shovel");
        }
        if (watering_can == null)
        {
            watering_can = GameObject.Find("player/character-male-b/root/torso/arm-left/watering_can");
        }
        if (sickle == null)
        {
            sickle = GameObject.Find("player/character-male-b/root/torso/arm-left/sickle");
        }
        if (carrot == null)
        {
            carrot = GameObject.Find("player/character-male-b/root/torso/arm-left/carrot");
        }
        if (carrot != null)
        {
            carrotScript = carrot.GetComponent<Carrot>();
        }
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
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
        transform.position = new Vector3(currentPosition.x, -3f, currentPosition.z);

        // checks if they player is in farming mode
        if (Input.GetKeyDown(KeyCode.E))
        {
            isFarmingMode = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) && !uiController.IsNightPhase)
        {
            isFarmingMode = true;
        }

        // checks the phase
        if (uiController.IsNightPhase)
        {
            isFarmingMode = false;
            progressCanvas.gameObject.SetActive(false); // Hide progress circle
            growingPhase = 0;   // Reset phase
            if (playerAnimator != null)
                playerAnimator.SetBool("isPlanting", false);
            if (playerAnimator != null)
                playerAnimator.SetBool("isWatering", false);
            if (playerAnimator != null)
                playerAnimator.SetBool("isHarvesting", false);
            if (shovel != null)
                shovel.SetActive(false);
            if (watering_can != null)
                watering_can.SetActive(false);
            if (sickle != null)
                sickle.SetActive(false);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            StopAllCoroutines();
            ResetPlot();        // Reset plot
        } 

        // checks fly health
        //if (plantHealth <= 0 && plantActive)
        //{
        //    growing = false;
        //    growingPhase = 0;
        //    StopAllCoroutines();
        //    NotifyFly();
        //    ResetPlot();
        //}

        if (FarmManager.IsHolding)
        {
            FarmManager.IsAnimationPlaying = true;
        }
    }

    // Player interacts with plot
    private void OnMouseDown()
    {
        // Stops interaction if an animation is playing on another plot
        if (FarmManager.IsAnimationPlaying)
            return;

        // Checks if planting has started and growth is not in progress and if the player is in close proximity
        if (isFarmingMode && !growing && player != null)
        {
            // Only compares X and Z distance
            Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 plotXZ = new Vector3(transform.position.x, 0, transform.position.z);
            float distance = Vector3.Distance(playerXZ, plotXZ);

            if (distance <= interactionRange) // Check if player is close enough
            {
                progressCanvas.gameObject.SetActive(true);
                StartCoroutine(HandleGrowth());
            }
        }
    }

    public void StartGrowthByHelper()
    {
        if (!growing)
        {
            progressCanvas.gameObject.SetActive(true);
            NPCFarming = true;
            StartCoroutine(HandleGrowth());
        }
    }

    private IEnumerator HandleGrowth()
    {
        growing = true;

        if (growingPhase == 0)   // Planting Phase
        {
            if(NPCFarming)
            {
                // Reset health
                plantHealth = 20;
                plantActive = true;

                // Timer for 3 seconds for planting animation
                yield return StartCoroutine(FillBar(0.25f, 1.5f));

                plantStem.SetActive(true);  // Stem asset appears
                growingPhase++;  // Move to next phase
                NPCFarming = false;
            }
            else
            {
                // Reset health
                plantHealth = 20;
                plantActive = true;

                // Planting shovel animation
                if (playerAnimator != null)
                    playerAnimator.SetBool("isPlanting", true);

                FarmManager.IsAnimationPlaying = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;

                if (shovel != null)
                    shovel.SetActive(true);

                // Timer for 3 seconds for planting animation
                yield return StartCoroutine(FillBar(0.25f, 1.5f));

                plantStem.SetActive(true);  // Stem asset appears
                growingPhase++;  // Move to next phase

                // Hide the shovel after planting is done
                if (shovel != null)
                    shovel.SetActive(false);

                // Reset the attack animation and unlock movement
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("isPlanting", false);
                }
                FarmManager.IsAnimationPlaying = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
        else if (growingPhase == 1)  // Watering Phase
        {
            if(NPCFarming)
            {
                // Timer for 5 seconds
                yield return StartCoroutine(FillBar(0.5f, 2f));
                plantStem.SetActive(false);
                halfPlant.SetActive(true);    // Half plant asset appears

                StartCoroutine(GrowthPhase());  // Growing starts without user interaction
                                                // Wait for growth phase
                yield return new WaitUntil(() => !growing);
                harvestGrowth = false;
                growingPhase++;    // Move to next phase
                NPCFarming = false;
            }
            else
            {
                // Watering animation
                if (playerAnimator != null)
                    playerAnimator.SetBool("isWatering", true);

                FarmManager.IsAnimationPlaying = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;

                if (watering_can != null)
                    watering_can.SetActive(true);

                // Timer for 5 seconds
                yield return StartCoroutine(FillBar(0.5f, 2f));
                plantStem.SetActive(false);
                halfPlant.SetActive(true);    // Half plant asset appears

                if (watering_can != null)
                    watering_can.SetActive(false);

                // Reset animation
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("isWatering", false);
                }

                FarmManager.IsAnimationPlaying = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

                StartCoroutine(GrowthPhase());  // Growing starts without user interaction
                                                // Wait for growth phase
                yield return new WaitUntil(() => !growing);
                growingPhase++;    // Move to next phase
            }
        }
        else if (growingPhase == 2)  // Harvesting Phase
        {
            // Harvesting animation
            if (playerAnimator != null)
                playerAnimator.SetBool("isHarvesting", true);

            FarmManager.IsAnimationPlaying = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            if (sickle != null)
                sickle.SetActive(true);

            // Timer for 3 seconds
            yield return StartCoroutine(FillBar(0f, 1.5f));

            if (sickle != null)
                sickle.SetActive(false);

            // Reset animation
            if (playerAnimator != null)
                playerAnimator.SetBool("isHarvesting", false);

            FarmManager.IsAnimationPlaying = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            fullPlant.SetActive(false);
            progressCanvas.gameObject.SetActive(false); // Hide progress circle
            growingPhase = 0;   // Reset phase
            ResetPlot();        // Reset plot

            if (carrot != null)
            {
                carrot.SetActive(true);

                // palyer holds animation
                if (carrotScript != null)
                {
                    carrotScript.StartHoldingCarrot();
                }
            }
        }
        growing = false;
    }

    private IEnumerator GrowthPhase()
    {
        harvestGrowth = true;
        growing = true;
        // Timer for 4 seconds
        yield return StartCoroutine(FillBar(1f, 3f));
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
        plantActive = false;
    }

    // Fly interactions
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FlySwarm"))
        {
            // Check if 5 seconds have passed
            if (Time.time - lastDamageTime >= damageInterval)
            {
                plantHealth -= 5;
                lastDamageTime = Time.time;

                // Reset plot if health reaches 0
                if (plantHealth <= 0 && plantActive)
                {
                    growing = false;
                    growingPhase = 0;   // Reset phase
                    StopAllCoroutines();  
                    ResetPlot();        // Reset plot
                    NotifyFly();
                }
            }
        }
    }

    private void NotifyFly()
    {
        FlyAI fly = FindObjectOfType<FlyAI>();
        if (fly != null)
        {
            fly.OnPlantDestroyed(this.gameObject);
        }
    }
}