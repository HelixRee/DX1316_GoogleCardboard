using UnityEngine;

public class BallFetch : MonoBehaviour
{
    private Rigidbody rb;
    private Transform playerCamera;
    public bool isHeldByPlayer = false;
    private PetMover dogScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main.transform;
        dogScript = FindFirstObjectByType<PetMover>();
    }

    public void OnPointerClick()
    {
        if (!isHeldByPlayer)
        {
            GrabBall();
        }
        else
        {
            ThrowBall();
        }
    }

    void GrabBall()
    {
        isHeldByPlayer = true;
        rb.isKinematic = true; // Physics off so it doesn't fall
        transform.SetParent(playerCamera);
        transform.localPosition = new Vector3(0, 0.0f, 0.75f); // Position in view
    }

    void ThrowBall()
    {
        isHeldByPlayer = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.linearVelocity = playerCamera.forward * 12f; // Launch forward

        if (dogScript != null)
        {
            dogScript.FetchBall(this.transform);
        }
    }
}