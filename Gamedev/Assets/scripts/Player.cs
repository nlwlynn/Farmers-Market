using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        isWalking = moveDir != Vector3.zero;

        // Move the player using Rigidbody
        if (moveDir != Vector3.zero)
        {
            // Normalize the move direction to prevent faster diagonal movement
            moveDir.Normalize();

            // Apply movement using Rigidbody
            rb.velocity = moveDir * moveSpeed;

            // Rotate the player smoothly
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.fixedDeltaTime * rotateSpeed);
        }
        else
        {
            // Stop movement if no input
            rb.velocity = Vector3.zero;
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}