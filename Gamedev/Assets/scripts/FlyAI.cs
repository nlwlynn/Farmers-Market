using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAI : MonoBehaviour
{
    public GameObject fly; 
    public float spawnRadius = 5f;
    public float attackRange = 3f;
    public float moveSpeed = 2f;
    public int health = 20;
    public UIController uiController;
    public Transform spawnPoint;
    private bool startedDay = false;
    private bool reactivateFly = false;

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
        // start with active flies
        if (fly != null)
        {
            fly.SetActive(true); 
        }
    }

    void Update()
    {
        // checks if night phase to activate or deactivate flies
        if (uiController.isNightPhase)
        {
            DeactivateFly();
        }
        else
        {
            ActivateFly();
            FindTargetCrop();
            MoveTowardsTarget();
        }
    }

    void FindTargetCrop()
    {
        int highestGrowthPhase = -1;  
        int highestValue = -1;        
        GameObject bestCrop = null;

        // loops through all the crops
        foreach (var cropValue in cropValues)
        {
            GameObject[] crops = GameObject.FindGameObjectsWithTag(cropValue.Key);

            // loops through all the crops in the scene
            foreach (GameObject crop in crops)
            {
                Component cropGrowthScript = null;
                int currentGrowthPhase = 0;

                // gets the growth phase of eat crop plot
                switch (cropValue.Key)
                {
                    case "Carrot":
                        cropGrowthScript = crop.GetComponent<CarrotGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((CarrotGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    case "Broccoli":
                        cropGrowthScript = crop.GetComponent<BroccoliGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((BroccoliGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    case "Cauliflower":
                        cropGrowthScript = crop.GetComponent<CauliflowerGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((CauliflowerGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    case "Mushroom":
                        cropGrowthScript = crop.GetComponent<LettuceGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((LettuceGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    case "Corn":
                        cropGrowthScript = crop.GetComponent<PumpkinGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((PumpkinGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    case "Sunflower":
                        cropGrowthScript = crop.GetComponent<WatermelonGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((WatermelonGrowth)cropGrowthScript).growingPhase;
                        }
                        break;
                    default:
                        continue;
                }

                if (cropGrowthScript == null) continue;

                int currentValue = cropValues[crop.tag];

                // proritizes crop growth over crop value
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
        if (targetCrop == null || fly == null) return;

        // Finds the direction to the crop
        Vector3 directionToTarget = targetCrop.transform.position - fly.transform.position;

        // Stops the fly from sinking
        if (fly.transform.position.y < 4) 
        {
            fly.transform.position = new Vector3(fly.transform.position.x, 4, fly.transform.position.z);
        }

        // Move the fly to the crop
        fly.transform.position = Vector3.MoveTowards(fly.transform.position, targetCrop.transform.position, moveSpeed * Time.deltaTime);
        fly.transform.position = new Vector3(fly.transform.position.x, Mathf.Max(fly.transform.position.y, 0f), fly.transform.position.z);
    }

    // Hide fly
    void DeactivateFly()
    {
        if (fly != null)
        {
            fly.GetComponent<Renderer>().enabled = false;
            fly.GetComponent<Collider>().enabled = false;
        }
        startedDay = false;
    }

    // Show fly
    void ActivateFly()
    {
        if (fly != null)
        {
            if (!startedDay || reactivateFly)
            {
                fly.transform.position = spawnPoint.position;
                startedDay = true;
                reactivateFly = false;
                health = 20;
            }

            fly.GetComponent<Renderer>().enabled = true;
            fly.GetComponent<Collider>().enabled = true;
        }
    }


    // Damage fly
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // Fly dies when reaching 0 health
    private void Die()
    {
        DeactivateFly();
        reactivateFly = true;
    }

    // Fly moves away from destroyed plant
    public void OnPlantDestroyed(GameObject destroyedPlant)
    {
        StartCoroutine(MoveAwayFromDestroyedPlant(destroyedPlant));
    }

    // Moves the fly
    private IEnumerator MoveAwayFromDestroyedPlant(GameObject destroyedPlant)
    {
        // Finds direction to move
        Vector3 awayFromPlant = fly.transform.position - destroyedPlant.transform.position;

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
            fly.transform.position += awayFromPlant * moveSpeed * Time.deltaTime;

            // Fixes the y value for fly to float
            Vector3 currentPosition = fly.transform.position;
            currentPosition.y = 4;
            fly.transform.position = currentPosition;

            timeElapsed += Time.deltaTime;
            moveSpeed = 10f;
            yield return null;
        }
    }

}
