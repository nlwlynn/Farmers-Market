using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_HOLDING_SPRAY = "IsHoldingSpray";

    [SerializeField] private Player player;
    [SerializeField] private GameObject sprayBottle;
    [SerializeField] private Transform handTransform;

    private Animator animator;
    private bool isHoldingSpray = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (sprayBottle != null)
        {
            sprayBottle.SetActive(false);
        }
    }

    private void Update()
    {
        bool isWalking = player.IsWalking();
        animator.SetBool(IS_WALKING, isWalking);

        // attack mode when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHoldingSpray = true;

            if (sprayBottle != null && !sprayBottle.activeSelf)
            {
                sprayBottle.SetActive(true);
            }
        }

        // farm mode when Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isHoldingSpray = false;
            animator.SetBool(IS_HOLDING_SPRAY, false);

            if (sprayBottle != null)
            {
                sprayBottle.SetActive(false);
            }
        }

        // player aims when theyre not walking
        if (!isWalking && isHoldingSpray)
        {
            animator.SetBool(IS_HOLDING_SPRAY, true);
        }
    }
}