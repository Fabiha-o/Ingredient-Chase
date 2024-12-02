using UnityEngine;

public class Checkpoint2 : MonoBehaviour
{
    // Static variable to hold the respawn position for Ella
    private static Vector3 respawnPosition;

    // Public property to access respawnPosition from outside the class
    public static Vector3 RespawnPosition
    {
        get => respawnPosition;
        set => respawnPosition = value;
    }

    // Called when another object enters the trigger collider (e.g., when Ella reaches the checkpoint)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that collided is Ella (using the GameObject name)
        if (other.gameObject.name == "Ella")
        {
            // Update the respawn position to this checkpoint's position
            respawnPosition = transform.position;
            Debug.Log("Ella's respawn position updated to: " + respawnPosition);

            // You could save the checkpoint position here if needed (e.g., using PlayerPrefs)
            SaveProgress();
        }
    }

    // Save the checkpoint progress (optional for persistent progress between game sessions)
    private void SaveProgress()
    {
        // Use PlayerPrefs to save the respawn position
        PlayerPrefs.SetFloat("EllaCheckpointX", respawnPosition.x);
        PlayerPrefs.SetFloat("EllaCheckpointY", respawnPosition.y);
        PlayerPrefs.SetFloat("EllaCheckpointZ", respawnPosition.z);
        PlayerPrefs.Save();

        Debug.Log("Ella's checkpoint progress saved.");
    }

    // Load the saved progress (respawn position) when the game starts or when needed
    public static void LoadProgress()
    {
        if (PlayerPrefs.HasKey("EllaCheckpointX"))
        {
            // Retrieve the saved respawn position
            float x = PlayerPrefs.GetFloat("EllaCheckpointX");
            float y = PlayerPrefs.GetFloat("EllaCheckpointY");
            float z = PlayerPrefs.GetFloat("EllaCheckpointZ");

            respawnPosition = new Vector3(x, y, z);

            Debug.Log("Ella's checkpoint progress loaded: " + respawnPosition);
        }
        else
        {
            // If no saved checkpoint, default to the starting position (e.g., origin or some default location)
            respawnPosition = new Vector3(0, 0, 0); // Modify based on your game's default start position
            Debug.Log("No saved checkpoint found. Using default start position.");
        }
    }
}
