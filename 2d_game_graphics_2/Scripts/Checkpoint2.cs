using UnityEngine;

public class Checkpoint2 : MonoBehaviour
{
    // Called when Ella reaches the checkpoint
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Ella")
        {
            // Update the checkpoint position for Ella
            Ella ellaScript = other.GetComponent<Ella>();
            if (ellaScript != null)
            {
                ellaScript.SetCheckpoint(transform.position);  // Set checkpoint in Ella's script
                SaveProgress(transform.position);  // Save the checkpoint progress
            }
        }
    }

    // Save the checkpoint position for Ella to PlayerPrefs
    private void SaveProgress(Vector3 checkpointPosition)
    {
        PlayerPrefs.SetFloat("EllaCheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat("EllaCheckpointY", checkpointPosition.y);
        PlayerPrefs.SetFloat("EllaCheckpointZ", checkpointPosition.z);
        PlayerPrefs.Save();

        Debug.Log("Ella's checkpoint progress saved.");
    }

    // Load the saved progress (respawn position) when the game starts or when needed
    public static Vector3 LoadProgress()
    {
        if (PlayerPrefs.HasKey("EllaCheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("EllaCheckpointX");
            float y = PlayerPrefs.GetFloat("EllaCheckpointY");
            float z = PlayerPrefs.GetFloat("EllaCheckpointZ");

            Vector3 respawnPosition = new Vector3(x, y, z);
            Debug.Log("Ella's checkpoint progress loaded: " + respawnPosition);
            return respawnPosition;
        }
        else
        {
            // No saved checkpoint, default to start position
            Debug.Log("No saved checkpoint found. Using default start position.");
            return Vector3.zero;  // Modify as necessary for your game start position
        }
    }
}
