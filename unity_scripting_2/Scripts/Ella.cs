using UnityEngine;

public class Ella : MonoBehaviour
{
    // This method will be called when the game starts or when Ella respawns
    void Start()
    {
        // Load the saved checkpoint position for Ella from the Checkpoint2 script
        Checkpoint2.LoadProgress();

        // Set Ella's position to the saved checkpoint position (if any)
        transform.position = Checkpoint2.RespawnPosition;

        Debug.Log("Ella's position set to the checkpoint: " + transform.position);
    }

    // This method will be called when Ella needs to respawn (e.g., if health reaches zero)
    public void Respawn()
    {
        // Load the checkpoint when Ella respawns
        Checkpoint2.LoadProgress();

        // Set Ella's position to the saved checkpoint position
        transform.position = Checkpoint2.RespawnPosition;

        Debug.Log("Ella respawned at: " + transform.position);
    }
}
