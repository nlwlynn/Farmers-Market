using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_HOLDING_SPRAY = "IsHoldingSpray";

    [SerializeField] private Player player;
    [SerializeField] private GameObject sprayBottle;  // Spray Bottle GameObject
    [SerializeField] private GameObject sprayBottleUpgrade;  // Spray Bottle GameObject
    [SerializeField] private Transform handTransform;  // Hand position to hold the spray bottle
    [SerializeField] private GameObject bulletPrefab;  // Bullet prefab to be fired
    [SerializeField] private GameObject bulletUpgradePrefab;  // Bullet prefab to be fired
    [SerializeField] private float bulletSpeed = 10f;  // Bullet speed
    [SerializeField] private Animator playerAnimator;  // Animator reference
    [SerializeField] private bool notPurchased = false; // to track here  

    private Animator animator;
    private bool isHoldingSpray = false;
    public bool upgradePurchased = false;   // shop variable

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (sprayBottle != null)
        {
            sprayBottle.SetActive(false);  // Start the spray bottle inactive
        }
        if (sprayBottleUpgrade != null)
        {
            sprayBottleUpgrade.SetActive(false);  // Start the spray bottle inactive
        }
    }

    private void Update()
    {
        bool isWalking = player.IsWalking();
        animator.SetBool(IS_WALKING, isWalking);


        if (sprayBottle != null && upgradePurchased && !notPurchased)
        {
            sprayBottle.SetActive(false);
            isHoldingSpray = false;
            animator.SetBool(IS_HOLDING_SPRAY, false);
            notPurchased = true;
        }

        // switches spray bottle mode when E is pressed
        if (Input.GetKeyDown(KeyCode.E) && !(FarmManager.IsHolding || FarmManager.IsAnimationPlaying))
        {
            isHoldingSpray = true;

            if (sprayBottle != null && !sprayBottle.activeSelf && !upgradePurchased)
            {
                sprayBottle.SetActive(true);
            }
            else if (sprayBottleUpgrade != null && !sprayBottleUpgrade.activeSelf && upgradePurchased)
            {
                sprayBottleUpgrade.SetActive(true);
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
            if (sprayBottleUpgrade != null)
            {
                sprayBottleUpgrade.SetActive(false);
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

    // the attack animation will play
    public void TriggerAttackAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("attack-melee-left");  
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && sprayBottle != null && !upgradePurchased)
        {
            // issue with getting it to look like it was firing from the gun
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
        else if (bulletUpgradePrefab != null && sprayBottleUpgrade != null && upgradePurchased)
        {
            // issue with getting it to look like it was firing from the gun
            Vector3 spawnPosition = sprayBottleUpgrade.transform.position + Vector3.up * 0.5f;

            GameObject bullet = Instantiate(bulletUpgradePrefab, spawnPosition, Quaternion.identity);

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

    public void ResetPestisideUpgrades(bool state)
    {
        sprayBottle.SetActive(state);
        isHoldingSpray = state;
        animator.SetBool(IS_HOLDING_SPRAY, state);
        upgradePurchased = state;
        notPurchased = state;
        sprayBottleUpgrade.SetActive(state);  
    }

    // shop variable for purchasing
    public void PurchasedInShop()
    {
        upgradePurchased = true;
    }
}
