using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_HOLDING_SPRAY = "IsHoldingSpray";

    [SerializeField] private Player player;
    [SerializeField] private GameObject sprayBottle;  // Spray Bottle GameObject
    [SerializeField] private Transform handTransform;  // Hand position to hold the spray bottle
    [SerializeField] private GameObject bulletPrefab;  // Bullet prefab to be fired
    [SerializeField] private float bulletSpeed = 10f;  // Bullet speed

    private Animator animator;
    private bool isHoldingSpray = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (sprayBottle != null)
        {
            sprayBottle.SetActive(false);  // Start the spray bottle inactive
        }
    }

    private void Update()
    {
        bool isWalking = player.IsWalking();
        animator.SetBool(IS_WALKING, isWalking);

        // switches spray bottle mode when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHoldingSpray = true;

            if (sprayBottle != null && !sprayBottle.activeSelf)
            {
                sprayBottle.SetActive(true);  
            }
        }

        // switches to farming mode when q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isHoldingSpray = false;
            animator.SetBool(IS_HOLDING_SPRAY, false);

            if (sprayBottle != null)
            {
                sprayBottle.SetActive(false);  
            }
        }

        // player can only fire when gun is held
        if (!isWalking && isHoldingSpray)
        {
            animator.SetBool(IS_HOLDING_SPRAY, true);

            // left mode triggers spray bottle bullets
            if (Input.GetMouseButtonDown(0)) 
            {
                FireBullet();
            }
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && sprayBottle != null)
        {
            // issue with getting it to look like it was firing form the gun
            Vector3 spawnPosition = sprayBottle.transform.position + Vector3.up * 0.5f;

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // bullets fire in the direction of the players
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = transform.forward * bulletSpeed;  
            }

            bullet.transform.rotation = Quaternion.LookRotation(transform.forward); 

            // bullets are destroyed after 3 seconds
            Destroy(bullet, 3f);
        }
    }
}

