using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperNPC : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of forward movement
    public float jumpInterval = 3f; // Time between jumps
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        StartCoroutine(JumpRoutine()); // Start jump coroutine
    }

    void Update()
    {
        MoveForward();
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        animator.SetBool("isMoving", true); // Set walking animation
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpInterval);
            Jump();
        }
    }

    void Jump()
    {
        animator.SetTrigger("Jump"); // Trigger jump animation
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // Apply jump force
    }
}
