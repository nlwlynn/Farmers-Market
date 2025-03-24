using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public enum SlimeAnimationSt { Idle, Walk, Jump, Attack, Damage }

public class HelperNPC : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public int damType;

    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;

    public enum WalkType { ToOrigin }
    private WalkType walkType;
    public bool atCrop = false;
    public UIController uiController;
    private bool isMovingAway = false;

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
        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        walkType = WalkType.ToOrigin;

        // Ensure NavMeshAgent is correctly set up
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if(uiController.IsNightPhase)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        // Look for target crop if none is assigned
        if (targetCrop == null || atCrop == false)
        {
            FindTargetCrop();
        }

        // Move towards the target crop
        if (currentState == SlimeAnimationState.Walk && !isMovingAway)
        {
            MoveToTargetCrop();
        }

        // Handle animation transitions
        switch (currentState)
        {
            case SlimeAnimationState.Idle:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;

            case SlimeAnimationState.Walk:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;
                agent.isStopped = false;
                agent.updateRotation = true;
                SetFace(faces.WalkFace);
                break;

            case SlimeAnimationState.Jump:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");
                break;

            case SlimeAnimationState.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;

            case SlimeAnimationState.Damage:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0") || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;
                StopAgent();
                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", damType);
                SetFace(faces.damageFace);
                break;
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

                // gets the growth phase of each crop
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

                // prioritizes crop growth over crop value
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

    void MoveToTargetCrop()
    {
        if (targetCrop != null)
        {
            // Ignore Y by keeping NPC's current Y level
            Vector3 targetPosition = targetCrop.transform.position;
            targetPosition.y = transform.position.y; // Keep NPC's current Y position

            // Move towards target while ignoring Y
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

            // Get direction (ignoring Y)
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Ignore vertical rotation

            // Rotate towards target only if there is movement
            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Check if NPC has reached the crop (ignoring Y)
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                 new Vector3(targetCrop.transform.position.x, 0, targetCrop.transform.position.z)) < 1f)
            {
                atCrop = true;
                InteractWithCrop();
            }
        }
    }

    void InteractWithCrop()
    {
        if (targetCrop == null || isMovingAway) return;

        // Determine the crop type and access its growth script
        CarrotGrowth cropGrowthScript = targetCrop.GetComponent<CarrotGrowth>();
        if (cropGrowthScript != null)
        {
            // Check the current growth phase and start the appropriate coroutine
            int currentGrowthPhase = cropGrowthScript.growingPhase;

            if (currentGrowthPhase == 2) return;

            // Start the growth process for the crop
            cropGrowthScript.StartGrowthByHelper();

            // Set NPC to Idle state while waiting
            currentState = SlimeAnimationState.Idle;
            animator.SetFloat("Speed", 0); // Make sure the NPC is not moving

            switch (currentGrowthPhase)
            {
                case 0:
                    StartCoroutine(WaitAndMoveAway(1.5f)); // Coroutine for waiting and moving away
                    break;
                case 1:
                    StartCoroutine(WaitAndMoveAway(2.0f)); // Coroutine for waiting and moving away
                    break;
                default:
                    StartCoroutine(WaitAndMoveAway(1.0f)); // Default wait time
                    break;
            }
        }
    }

    // Coroutine to handle waiting and then moving away from the crop
    IEnumerator WaitAndMoveAway(float waitTime)
    {
        currentState = SlimeAnimationState.Jump;

        // Wait for the specified amount of time while the NPC is idle
        yield return new WaitForSeconds(waitTime);

        // After the wait, move the NPC away from the crop
        MoveAwayFromCrop();
    }

    // Move the NPC away from the crop (already in your previous code)
    void MoveAwayFromCrop()
    {
        if (targetCrop == null) return;

        currentState = SlimeAnimationState.Walk;
        isMovingAway = true;
        atCrop = false;

        // Get the direction away from the crop (opposite direction of target crop)
        Vector3 moveDirection = (transform.position - targetCrop.transform.position).normalized;

        // Define a distance to move away
        float moveDistance = 50f;

        // Calculate the target position
        Vector3 targetPosition = transform.position + moveDirection * moveDistance;

        // Move the NPC to the new position
        StartCoroutine(MoveNPCToPosition(targetPosition));
    }

    IEnumerator MoveNPCToPosition(Vector3 targetPosition)
    {
        float duration = 1f; // Duration for the movement
        Vector3 startPosition = transform.position;
        float timeElapsed = 0f;

        // Smoothly move the NPC towards the target position
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure NPC ends at the target position
        transform.position = targetPosition;

        // After moving away, allow the NPC to resume normal movement
        isMovingAway = false;
        targetCrop = null;
    }


    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            // Handle damage animation ending
            float distanceOrg = Vector3.Distance(transform.position, originPos);
            if (distanceOrg > 1f)
            {
                walkType = WalkType.ToOrigin;
                currentState = SlimeAnimationState.Walk;
            }
            else currentState = SlimeAnimationState.Idle;
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }
    }

    void OnAnimatorMove()
    {
        // Apply root motion to NPC
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}
