using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAI : MonoBehaviour
{
    public float spawnRadius = 10f;
    public float attackRange = 3f;
    public float moveSpeed = 15f;
    public UIController uiController;
    public Transform spawnPoint;
    private bool startedDay = false;
    private bool reactivateCrow = false;
    private Vector3 targetOffset;
    private Animator animator;
    private bool hasPecked = false;



    private Dictionary<string, int> cropValues = new Dictionary<string, int>
    {
        {"Carrot", 1}, {"Broccoli", 2}, {"Cauliflower", 3},
        {"Mushroom", 4}, {"Corn", 5}, {"Sunflower", 6}
    };
    private GameObject targetCrop;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
    }

    void Start()
    {
 
        gameObject.SetActive(true);
        targetOffset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
        animator = GetComponent<Animator>();


    }

    void Update()
    {
        // checks if night phase to activate or deactivate flies
        if (uiController.isNightPhase)
        {
            DeactivateCrow();
            startedDay = false;
        }
        else
        {
            ActivateCrow();
            FindTargetCrop();
            MoveTowardsTarget();

            if (animator != null && targetCrop != null)
            {
                float distance = Vector3.Distance(transform.position, targetCrop.transform.position + targetOffset);
                bool isFlying = distance > 0.5f;
                animator.SetBool("flying", isFlying);

                if (!isFlying && !hasPecked)
                {
                    animator.SetTrigger("peck");  // trigger your peck animation
                    hasPecked = true;
                }
                else if (isFlying && hasPecked)
                {
                    hasPecked = false;
                }
            }
        }
    }

    void FindTargetCrop()
    {
        int highestGrowthPhase = -1;
        int highestValue = -1;
        GameObject bestCrop = null;

        foreach (var cropValue in cropValues)
        {
            GameObject[] crops = GameObject.FindGameObjectsWithTag(cropValue.Key);

            foreach (GameObject crop in crops)
            {
                CropProtection protection = crop.GetComponent<CropProtection>();
                if (protection != null && protection.IsProtected())
                {
                    Debug.Log($"Skipping protected crop: {crop.name}");
                    continue;
                }


                Component cropGrowthScript = null;
                int currentGrowthPhase = 0;

                switch (cropValue.Key)
                {
                    case "Carrot":
                        cropGrowthScript = crop.GetComponent<CarrotGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((CarrotGrowth)cropGrowthScript).growingPhase;
                        break;
                    case "Broccoli":
                        cropGrowthScript = crop.GetComponent<BroccoliGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((BroccoliGrowth)cropGrowthScript).growingPhase;
                        break;
                    case "Cauliflower":
                        cropGrowthScript = crop.GetComponent<CauliflowerGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((CauliflowerGrowth)cropGrowthScript).growingPhase;
                        break;
                    case "Mushroom":
                        cropGrowthScript = crop.GetComponent<LettuceGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((LettuceGrowth)cropGrowthScript).growingPhase;
                        break;
                    case "Corn":
                        cropGrowthScript = crop.GetComponent<PumpkinGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((PumpkinGrowth)cropGrowthScript).growingPhase;
                        break;
                    case "Sunflower":
                        cropGrowthScript = crop.GetComponent<WatermelonGrowth>();
                        if (cropGrowthScript != null)
                            currentGrowthPhase = ((WatermelonGrowth)cropGrowthScript).growingPhase;
                        break;
                }

                if (cropGrowthScript == null) continue;

                int currentValue = cropValues[crop.tag];

    if (currentGrowthPhase > highestGrowthPhase ||
        (currentGrowthPhase == highestGrowthPhase && currentValue > highestValue))
    {
        highestGrowthPhase = currentGrowthPhase;
        highestValue = currentValue;
        bestCrop = crop;
    }
            }
        }

        targetCrop = bestCrop;
    }




    void MoveTowardsTarget()
    {
        if (targetCrop == null) return;

        Vector3 targetPosition = targetCrop.transform.position + targetOffset;

        // Make sure crow stays airborne
        if (transform.position.y < 4)
        {
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }

        // Move toward the crop + offset
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Clamp y to avoid sinking
        transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, 0f), transform.position.z);
    }

    // Hide crow
    void DeactivateCrow()
    {

            GetComponentInChildren<Renderer>().enabled = false;
            GetComponentInChildren<Collider>().enabled = false;
        
    }

    // Show crow
    void ActivateCrow()
    {
        if (!startedDay)
        {
            transform.position = spawnPoint.position;
            startedDay = true;
            // health = 20;
        }
        else if (startedDay && reactivateCrow)
        {
            transform.position = spawnPoint.position;
            // health = 20;
            reactivateCrow = false;
        }

        GetComponentInChildren<Renderer>().enabled = true;
        GetComponentInChildren<Collider>().enabled = true;
    }

    // crow moves away from destroyed plant
    public void OnPlantDestroyed(GameObject destroyedPlant)
    {
        StartCoroutine(MoveAwayFromDestroyedPlant(destroyedPlant));
    }

    // Moves the crow
    private IEnumerator MoveAwayFromDestroyedPlant(GameObject destroyedPlant)
    {
        // Finds direction to move
        Vector3 awayFromPlant = transform.position - destroyedPlant.transform.position;

        // Sets the y value
        awayFromPlant.y = 0f;
        awayFromPlant.Normalize();

        // Move distance and time
        float moveAwayDuration = 10f;
        float timeElapsed = 0f;

        while (timeElapsed < moveAwayDuration)
        {
            moveSpeed = 20f;
            // Moves to new position
            transform.position += awayFromPlant * moveSpeed * Time.deltaTime;

            // Fixes the y value for crow to float
            Vector3 currentPosition = transform.position;
            currentPosition.y = 4;
            transform.position = currentPosition;

            timeElapsed += Time.deltaTime;
            moveSpeed = 10f;
            yield return null;
        }
    }

}
