using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private Animator playerAnimator; 

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerAnimator = GetComponent<Animator>();  
    }

    public Vector2 GetMovementVectorNormalized()
    {
        if (playerAnimator != null && playerAnimator.GetBool("isPlanting"))
        {
            return Vector2.zero;  
        }

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
