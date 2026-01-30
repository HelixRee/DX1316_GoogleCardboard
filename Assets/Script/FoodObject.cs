using UnityEngine;

public class FoodObject : MonoBehaviour
{
    private Rigidbody rb;
    private PetFoodGameManager manager;
    public float speed = 2f;

    void Start()
    {
        manager = FindFirstObjectByType<PetFoodGameManager>();
        rb = GetComponent<Rigidbody>();

        // Give the bone an initial push in a random direction
        Vector3 randomDir = Random.onUnitSphere;
        rb.linearVelocity = randomDir * speed;
    }

    void FixedUpdate()
    {
        // Keep the speed consistent so it doesn't stop after bouncing
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    public void OnPointerClick()
    {
        if (manager != null)
        {
            manager.AddBone();
            transform.position = new Vector3(0, -100, 0);
            gameObject.SetActive(false);
        }
    }
}