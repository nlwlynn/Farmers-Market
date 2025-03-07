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
        if (fly != null)
        {
            fly.SetActive(true); 
        }
    }

    void Update()
    {
        Debug.Log("Current phase: " + (uiController.isNightPhase ? "Night" : "Day"));

        if (uiController.isNightPhase)
        {
           Debug.Log("fly night");
            DeactivateFly();
        }
        else
        {
            Debug.Log("fly day");
            ActivateFly();
            FindTargetCrop();
            MoveTowardsTarget();
        }
    }

    void FindTargetCrop()
    {
        int highestValue = 0;
        GameObject bestCrop = null;

        foreach (var cropValue in cropValues)
        {
            // Uses the crop name as the tag
            GameObject[] crops = GameObject.FindGameObjectsWithTag(cropValue.Key); 
            foreach (GameObject crop in crops)
            {
                // Targets crops that are more valuable
                if (cropValues[crop.tag] > highestValue)
                {
                    highestValue = cropValues[crop.tag];
                    bestCrop = crop;
                }

                // TODO: Target crops that have more damage
                // TODO: Target crops that are closer to being finished
                // TODO: make a new fly once one is killed
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
    }

    // Show fly
    void ActivateFly()
    {
        if (fly != null)
        {
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
        float moveAwayDuration = 20f; 
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
