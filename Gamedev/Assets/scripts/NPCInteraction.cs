using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI npcTextBox; // Text for orders above NPC head
    public Transform spawnPoint;  // Spawn position

    public float walkSpeed = 20f;
    public GameObject npcPrefab; // NPC model

    private Animator animator; // Reference to the Animator
    private bool orderComplete = false; // Flag to track order completion
    public UIController uiController; // Reference to UIController
    public int orderAmount = 0;
    public Transform leavePoint;

    private Dictionary<string, int> cropRewards = new Dictionary<string, int>
    {
        { "Carrot", 2 },
        { "Broccoli", 5 },
        { "Cauliflower", 10 },
        { "Sunflower", 17 },
        { "Corn", 12 },
        { "Mushrooms", 15 }
    };

    private List<string> requestedItems = new List<string>(); // What the NPC wants
    private List<string> deliveredItems = new List<string>(); // What we've given them so far

    private Coroutine customerRoutineCoroutine;

    void Start()
    {
        if (animator == null && npcPrefab != null)
        {
            animator = npcPrefab.GetComponent<Animator>();
        }

        // Start the customer order
        customerRoutineCoroutine = StartCoroutine(CustomerRoutine());
    }

    void Update()
    {
        // Check for night phase
        if (uiController.isNightPhase)
        {
            // Reset NPC state during night phase
            if (customerRoutineCoroutine != null)
            {
                StopCoroutine(customerRoutineCoroutine);
                customerRoutineCoroutine = null;
            }
            ResetNPC(); 
        }
        else if (!uiController.isNightPhase && customerRoutineCoroutine == null)
        {
            // Start the routine again on new day
            customerRoutineCoroutine = StartCoroutine(CustomerRoutine());
        }
    }

    IEnumerator CustomerRoutine()
    {
        while (true)
        {
            // Reset NPC state at the start
            ResetNPC();

            // Random delay before the NPC starts walking
            float randomDelay = Random.Range(1f, 5f);
            yield return new WaitForSeconds(randomDelay);

            // Walk to ordering position
            yield return TurnAndMove(0, 70f);

            // Generate Order & Display Text
            GenerateRequest();
            UpdateNPCText();

            float timer = 0f;

            // Wait for order completion or until timer runs out
            while (!orderComplete && timer < 30f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (!orderComplete)
            {
                npcTextBox.text = "Oh well!";
            }

            // NPC walks away after order completion
            yield return MoveToPosition(leavePoint.position);
            yield return TurnAndMove(-90, 70f);

            // Pause before respawning 
            float pauseTime = Random.Range(1f, 6f);
            yield return new WaitForSeconds(pauseTime);
        }
    }

    void ResetNPC()
    {
        // Stop the NPC and reset all relevant states
        transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
        npcTextBox.text = "";
        orderComplete = false;
        deliveredItems.Clear();
    }

    IEnumerator TurnAndMove(float turnAngle, float distance)
    {
        transform.Rotate(0, turnAngle, 0); // Rotate NPC
        Vector3 startPos = transform.position; // current pos
        Vector3 targetPos = startPos + transform.forward * distance;

        // Set walking animation
        animator.SetBool("isWalking", true);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop walking animation
        animator.SetBool("isWalking", false);
    }

    // Moves NPC to a specific position
    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        // Rotate towards the target direction
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;

        // Set walking animation
        animator.SetBool("isWalking", true);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop walking animation
        animator.SetBool("isWalking", false);
    }

    public void Interact(string item)
    {
        if (requestedItems.Contains(item))
        {
            requestedItems.Remove(item);
            deliveredItems.Add(item);
            UpdateNPCText();

            // Check if all requested items are delivered
            if (requestedItems.Count == 0)
            {
                CompleteOrder();
            }
        }
    }

    void CompleteOrder()
    {
        orderComplete = true;

        // Calculate order amount
        orderAmount = 0;
        foreach (string item in deliveredItems)
        {
            if (cropRewards.ContainsKey(item))
            {
                orderAmount += cropRewards[item];
            }
        }

        if (uiController != null)
        {
            UIController.Instance.AddCoins(orderAmount);
        }
    }

    void GenerateRequest()
    {
        requestedItems.Clear();
        string[] availableCrops = { "Carrot", "Broccoli", "Cauliflower", "Sunflower", "Corn", "Mushroom" };

        // Find available crops
        List<string> cropsInScene = new List<string>();
        foreach (string cropTag in availableCrops)
        {
            if (GameObject.FindGameObjectsWithTag(cropTag).Length > 0)
            {
                cropsInScene.Add(cropTag);
            }
        }

        // If there are crops in scene, request some
        if (cropsInScene.Count > 0)
        {
            int requestCount = Random.Range(1, Mathf.Min(5, cropsInScene.Count + 1));
            for (int i = 0; i < requestCount; i++)
            {
                string selectedCrop = cropsInScene[Random.Range(0, cropsInScene.Count)];
                requestedItems.Add(selectedCrop);
            }
        }
    }

    void UpdateNPCText()
    {
        npcTextBox.text = requestedItems.Count > 0
            ? "I need: " + string.Join(", ", requestedItems) + "!"
            : "Thank you!";
    }
}