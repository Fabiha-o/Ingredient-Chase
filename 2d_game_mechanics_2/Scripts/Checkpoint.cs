using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Static variable to hold the respawn position for ButlerX
    public static Vector3 RespawnPosition { get; set; }

    private void Start()
    {
        // Load saved respawn position or set to this checkpoint's position for ButlerX
        if (PlayerPrefs.HasKey("ButlerXRespawnX") && PlayerPrefs.HasKey("ButlerXRespawnY"))
        {
            RespawnPosition = new Vector3(
                PlayerPrefs.GetFloat("ButlerXRespawnX"),
                PlayerPrefs.GetFloat("ButlerXRespawnY"),
                0
            );
        }
        else
        {
            RespawnPosition = transform.position;  // Default to current checkpoint position
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When ButlerX hits the checkpoint, update the respawn position
        if (collision.gameObject.name == "ButlerX")
        {
            RespawnPosition = transform.position;

            // Save the respawn position to PlayerPrefs for ButlerX
            PlayerPrefs.SetFloat("ButlerXRespawnX", RespawnPosition.x);
            PlayerPrefs.SetFloat("ButlerXRespawnY", RespawnPosition.y);
            PlayerPrefs.Save();

            Debug.Log("ButlerX's checkpoint updated and saved!");
        }
    }
}
