using Unity.VisualScripting;
using UnityEngine;

public class PetFoodGameManager : MonoBehaviour
{
    [Header("Bones Spawning")]
    public GameObject bonePrefab;      // Drag your Bone Prefab here
    public Transform PetFoodParent;  // Center of the game room
    public Vector3 spawnRange = new Vector3(5, 2, 5); // Size of the room

    [Header("Locations")]
    public GameObject houseLocation;

    [Header("Game Progress")]
    public int bonesCollected = 0;
    public int targetBones = 8;
    public GameObject bowlBoneVisual;
    public TextMesh textMesh;

    void Start()
    {
        if (bowlBoneVisual != null) bowlBoneVisual.SetActive(false);
    }

    void OnEnable()
    {
        bonesCollected = 0;
        SpawnBones(); // Call the spawn function
        UpdateText();
    }

    void SpawnBones()
    {
        for (int i = 0; i < targetBones; i++)
        {
            // Calculate a random position within the room
            Vector3 randomPos = PetFoodParent.position + new Vector3(
                Random.Range(-spawnRange.x, spawnRange.x),
                Random.Range(0, spawnRange.y),
                Random.Range(-spawnRange.z, spawnRange.z)
            );

            Instantiate(bonePrefab, randomPos, Quaternion.identity, PetFoodParent);
        }
    }

    public void AddBone()
    {
        bonesCollected++;
        UpdateText();
        if (bonesCollected >= targetBones)
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        foreach (Transform child in PetFoodParent)
        {
            Destroy(child.gameObject , 2.0f);
        }

        if (houseLocation != null)
        {
            houseLocation.SetActive(true);
            if (bowlBoneVisual != null) bowlBoneVisual.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    void UpdateText()
    {
        if (textMesh != null)
        {
            // Change the text to show current progress
            textMesh.text = "Bones collected: " + bonesCollected + " / " + targetBones;
        }
    }
}