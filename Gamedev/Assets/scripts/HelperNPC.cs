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

    //public enum WalkType { ToOrigin }
    //private WalkType walkType;
    public bool atCrop = false;
    public UIController uiController;

    private Vector3 hiddenPosition = new Vector3(-1000, -1000, -1000);
    private bool isMovingAway = false;
    private bool toOrigin = false;
    public Transform spawnPoint;
    private string scriptNames = "";
    private string targetScript = "";
    private int interactionCount = 0;
    private bool waitToResp = false;

    public bool playerPurchased = true; // variable for the store
    private bool firstNight = true;

    private Dictionary<string, int> cropValues = new Dictionary<string, int>
    {
        {"Carrot", 1}, {"Broccoli", 2}, {"Cauliflower", 3},
        {"Mushroom", 4}, {"Corn", 5}, {"Sunflower", 6}
    };
    public GameObject targetCrop;

    private void Awake()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
    }

    void Start()
    {
        originPos = spawnPoint.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        //walkType = WalkType.ToOrigin;

        // Check NavMeshAgent
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (uiController.IsNightPhase || !playerPurchased || waitToResp)
        {
            //UnityEngine.Debug.Log("in looking for crop");

            transform.position = hiddenPosition; // Move in house
            toOrigin = false;
        }
        else if(!toOrigin)
        {
            UnityEngine.Debug.Log("in not to origin");

            agent.Warp(originPos); // Move to house
            toOrigin = true;
        }

        if (!uiController.IsNightPhase)
        {
            // Finds target crop
            if (targetCrop == null || atCrop == false)
            {
                //UnityEngine.Debug.Log("in looking for crop");
                FindTargetCrop();
            }

            // Move towards the target crop
            if (currentState == SlimeAnimationState.Walk && !isMovingAway && targetCrop != null)
            {
                MoveToTargetCrop();
            }
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
        bool isGrowing = false;
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
                bool growingStatus = false;

                // gets the growth phase of each crop
                switch (cropValue.Key)
                {
                    case "Carrot":
                        cropGrowthScript = crop.GetComponent<CarrotGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((CarrotGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((CarrotGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "CarrotGrowth";
                        }
                        break;
                    case "Broccoli":
                        cropGrowthScript = crop.GetComponent<BroccoliGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((BroccoliGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((BroccoliGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "BroccoliGrowth";
                        }
                        break;
                    case "Cauliflower":
                        cropGrowthScript = crop.GetComponent<CauliflowerGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((CauliflowerGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((CauliflowerGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "CauliflowerGrowth";
                        }
                        break;
                    case "Mushroom":
                        cropGrowthScript = crop.GetComponent<LettuceGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((LettuceGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((LettuceGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "LettuceGrowth";
                        }
                        break;
                    case "Corn":
                        cropGrowthScript = crop.GetComponent<PumpkinGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((PumpkinGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((PumpkinGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "PumpkinGrowth";
                        }
                        break;
                    case "Sunflower":
                        cropGrowthScript = crop.GetComponent<WatermelonGrowth>();
                        if (cropGrowthScript != null)
                        {
                            currentGrowthPhase = ((WatermelonGrowth)cropGrowthScript).growingPhase;
                            growingStatus = ((WatermelonGrowth)cropGrowthScript).harvestGrowth;
                            scriptNames = "WatermelonGrowth";
                        }
                        break;
                    default:
                        continue;
                }

                if (cropGrowthScript == null) continue;

                // makes sure it doesnt go to crops that are ready to be harvested
                if (isGrowing || currentGrowthPhase == 2) continue;

                int currentValue = cropValues[crop.tag];

                // prioritizes crop growth over crop value
                if (currentGrowthPhase > highestGrowthPhase ||
                    (currentGrowthPhase == highestGrowthPhase && currentValue > highestValue))
                {
                    highestGrowthPhase = currentGrowthPhase;
                    highestValue = currentValue;
                    bestCrop = crop;
                    isGrowing = growingStatus;
                    targetScript = scriptNames;
                }
            }
        }

        if (highestGrowthPhase == 2 || isGrowing == true)
        {
            targetCrop = null;
        }
        else
        {
            targetCrop = bestCrop;
        }
    }

    void MoveToTargetCrop()
    {
        if (targetCrop != null)
        {
            // Ignore y position
            Vector3 targetPosition = targetCrop.transform.position;
            targetPosition.y = transform.position.y;

            // Move to target 
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

            // Gets direction of target
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; 

            // Faces target
            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Check if helper is at the crop
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

        System.Type scriptType = System.Type.GetType(targetScript);

        if (scriptType != null)
        {
            Component cropGrowthScript = targetCrop.GetComponent(scriptType);
            if (cropGrowthScript != null)
            {
                var method = scriptType.GetMethod("StartGrowthByHelper");
                if (method != null)
                {
                    method.Invoke(cropGrowthScript, null); 
                }

                currentState = SlimeAnimationState.Idle;
                animator.SetFloat("Speed", 0); 

                var phaseProperty = scriptType.GetProperty("growingPhase");
                int currentGrowthPhase = 0;

                // checks growth phase is interactable
                if (phaseProperty != null)
                {
                    currentGrowthPhase = (int)phaseProperty.GetValue(cropGrowthScript);
                }

                if (currentGrowthPhase == 2) return;

                interactionCount++; 

                // allows the helper to interact with 3 crops at a time
                if (interactionCount >= 3)
                {
                    interactionCount = 0; 
                    StartCoroutine(WaitAndReturnToSpawn());
                } 
                else
                { 
                    // farming interactions
                    switch (currentGrowthPhase)
                    {
                        case 0:
                            StartCoroutine(WaitAndMoveAway(1.5f));
                            break;
                        case 1:
                            StartCoroutine(WaitAndMoveAway(2.0f));
                            break;
                        default:
                            StartCoroutine(WaitAndMoveAway(1.0f));
                            break;
                    }
                }
            }
        }
    }

    // returns helper back to house
    IEnumerator WaitAndReturnToSpawn()
    {
        currentState = SlimeAnimationState.Jump; 
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(MoveToSpawn());
    }

    IEnumerator MoveToSpawn()
    {
        if (spawnPoint == null)
        {
            yield break; 
        }

        currentState = SlimeAnimationState.Walk;
        isMovingAway = true;
        atCrop = false;

        // faces the direction of the house
        Vector3 directionToSpawn = (spawnPoint.position - transform.position).normalized;
        if (directionToSpawn != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToSpawn);
            transform.rotation = targetRotation; 
        }

        yield return StartCoroutine(MoveNPCToPosition(spawnPoint.position));

        // resting time for helper to respawn
        waitToResp = true;
        yield return new WaitForSeconds(6f);
        waitToResp = false;
    }

    // moves helper away from crop
    IEnumerator WaitAndMoveAway(float waitTime)
    {
        currentState = SlimeAnimationState.Jump;
        yield return new WaitForSeconds(waitTime);
        MoveAwayFromCrop();
    }

   
    void MoveAwayFromCrop()
    {
        if (targetCrop == null) return;

        currentState = SlimeAnimationState.Walk;
        isMovingAway = true;
        atCrop = false;

        // has the npc move in current direciton
        Vector3 moveDirection = transform.forward;

        float moveDistance = 10f;

        // finds new target
        Vector3 targetPosition = transform.position + moveDirection * moveDistance;

        StartCoroutine(MoveNPCToPosition(targetPosition));
    }

    IEnumerator MoveNPCToPosition(Vector3 targetPosition)
    {
        float duration = 5f; 
        Vector3 startPosition = transform.position;
        float timeElapsed = 0f;

        // moves the helper to target
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

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
                //walkType = WalkType.ToOrigin;
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

    public void ResetHelper(bool state)
    {
        playerPurchased = state;
        firstNight = true;
    }


    public void ChangeFirstNight(bool state)
    {
        firstNight = state;
    }

    public void ChangePlayerPurchased(bool state)
    {
        playerPurchased = state;
    }
}
