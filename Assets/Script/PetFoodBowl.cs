using UnityEngine;

public class PetFoodBowl : MonoBehaviour
{
    [Header("Teleport Settings")]
    public GameObject houseLocation;
    public GameObject gameLocation;

    public PetFoodGameManager manager; // The GameObject holding the script

    public void OnPointerClick()
    {
        if (houseLocation != null && gameLocation != null)
        {
            // Teleport the player to the game area
            houseLocation.SetActive(false);
            gameLocation.SetActive(true);

            if (manager != null)
            {
                manager.bonesCollected = 0; // Reset count for the new session
            }

            Debug.Log("Teleported to Pet Food Game!");
        }
    }
}