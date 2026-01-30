using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PetMover : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator anim;
    public float walkRadius = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("VFX Settings")]
    public GameObject heartVFXPrefab;

    [Header("Fetch Settings")]
    private Transform targetBall;
    private bool isFetching = false;
    private bool hasBall = false;

    private Transform playerCamera;
    private bool isGazedAt = false;
    private bool isBeingPetted = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        if (Camera.main != null) playerCamera = Camera.main.transform;

        agent.updatePosition = true;
        agent.updateRotation = true;

        // Roam will now check conditions before moving
        InvokeRepeating("Roam", 2f, 5f);
    }

    void Update()
    {
        if (anim == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        anim.SetFloat("Vert", speed > 0.1f ? 1f : 0f);
        anim.SetFloat("State", speed / agent.speed);

        if (isFetching)
        {
            HandleFetch();
        }
        else if (isGazedAt && playerCamera != null)
        {
            // Lock rotation logic here to prevent jitter
            FacePlayer();
        }
    }

    public void FetchBall(Transform ball)
    {
        targetBall = ball;
        isFetching = true;
        hasBall = false;
        isGazedAt = false;

        StartCoroutine(WaitAndFetch());
    }

    IEnumerator WaitAndFetch()
    {
        // 1. Wait for exactly 2 seconds
        yield return new WaitForSeconds(2.0f);
        // 2. CHECK: If the dog already caught the ball (straight throw), don't pathfind to the floor!
        if (hasBall)
        {
            Debug.Log("Dog already caught the ball! Skipping path to floor.");
            yield break;
        }

        // 3. Find the closest valid point on the NavMesh
        NavMeshHit hit;
        float searchRadius = 3.0f;

        if (NavMesh.SamplePosition(targetBall.position, out hit, searchRadius, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
            Debug.Log("Dog is now chasing the ball.");
        }
        else
        {
            isFetching = false;
            agent.isStopped = true;
        }
    }

    void HandleFetch()
    {
        if (targetBall == null) return;

        BallFetch ballScript = targetBall.GetComponent<BallFetch>();
        if (ballScript != null && ballScript.isHeldByPlayer)
        {
            isFetching = false;
            hasBall = false;
            agent.isStopped = true;
            return;
        }

        float distanceToBall = Vector3.Distance(transform.position, targetBall.position);

        if (!hasBall && distanceToBall < 1.0f && targetBall.parent == null)
        {
            hasBall = true;
            Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
            if (ballRb != null) ballRb.isKinematic = true;

            targetBall.SetParent(this.transform);
            targetBall.localPosition = new Vector3(0, 0.6f, 0.6f);
            agent.SetDestination(playerCamera.position);
        }

        if (hasBall)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);
            if (distanceToPlayer < 1.5f)
            {
                DropBall();
            }
        }
    }

    void DropBall()
    {
        isFetching = false;
        hasBall = false;
        targetBall.SetParent(null);

        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        if (ballRb != null) ballRb.isKinematic = false;

        agent.isStopped = true;
        Invoke("ResumeWalking", 2f);
    }

    void FacePlayer()
    {
        // Stop movement completely while rotating to face player
        if (agent.hasPath) agent.ResetPath();
        agent.isStopped = true;

        Transform objectToRotate = transform.parent != null ? transform.parent : transform;
        Vector3 direction = (playerCamera.position - objectToRotate.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            objectToRotate.rotation = Quaternion.Slerp(objectToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void Roam()
    {
        // CRITICAL FIX: Block Roam if player is interacting with dog
        if (isBeingPetted || isGazedAt || isFetching) return;

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void OnPointerEnter()
    {
        if (isFetching) return;
        isGazedAt = true;
        agent.isStopped = true;
    }

    public void OnPointerExit()
    {
        isGazedAt = false;
        if (!isBeingPetted && !isFetching) agent.isStopped = false;
    }

    public void OnPointerClick()
    {
        isBeingPetted = true;
        PetAction();
    }

    void PetAction()
    {
        agent.isStopped = true;
        anim.SetFloat("Vert", 0);
        anim.SetFloat("State", 0);

        if (heartVFXPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up;
            GameObject heart = Instantiate(heartVFXPrefab, spawnPos, heartVFXPrefab.transform.rotation);
            Destroy(heart, 2f);
        }

        Invoke("ResumeWalking", 3f);
    }

    void ResumeWalking()
    {
        isBeingPetted = false;
        if (!isGazedAt && !isFetching) agent.isStopped = false;
    }
}