using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_HOLDING_SPRAY = "IsHoldingSpray";

    [SerializeField] private Player player;
    [SerializeField] private GameObject sprayBottlePrefab;  // Reference to the spray bottle Prefab
    [SerializeField] private Transform handDummyTransform;   // The dummy position in the body mesh for the hand

    private Animator animator;
    private bool isHoldingSpray = false;  // Track whether the player is holding the spray
    private GameObject sprayBottleInstance; // The instance of the spray bottle in the scene

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update walking state based on player movement
        bool isWalking = player.IsWalking();
        animator.SetBool(IS_WALKING, isWalking);

        // Only allow spray holding animation when player stops walking and presses 'E'
        if (Input.GetKeyDown(KeyCode.E) && !isWalking) // If E is pressed and the player is not walking
        {
            isHoldingSpray = true;
            animator.SetBool(IS_HOLDING_SPRAY, isHoldingSpray);  // Switch to holding spray animation

            if (sprayBottleInstance == null)
            {
                sprayBottleInstance = Instantiate(sprayBottlePrefab);  // Instantiate the spray bottle
            }

            // Activate spray bottle and attach it to the dummy hand position
            sprayBottleInstance.SetActive(true);
            sprayBottleInstance.transform.SetParent(handDummyTransform);  // Attach to the hand dummy

            // Adjust position and rotation if needed (can be zero or slight offset)
            sprayBottleInstance.transform.localPosition = Vector3.zero;  // Adjust position if necessary
            sprayBottleInstance.transform.localRotation = Quaternion.identity;  // Adjust rotation if necessary
        }

        // If the player presses 'Q', stop holding spray
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isHoldingSpray = false;
            animator.SetBool(IS_HOLDING_SPRAY, isHoldingSpray);  // Switch to idle or walking animation
            if (sprayBottleInstance != null)
            {
                sprayBottleInstance.SetActive(false);  // Deactivate spray bottle
                sprayBottleInstance.transform.SetParent(null);  // Detach the spray bottle from the body
            }
        }
    }
}
