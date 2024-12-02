using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 RespawnPosition { get; set; }

    private void Start()
    {
        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY"))
        {
            RespawnPosition = new Vector3(
                PlayerPrefs.GetFloat("RespawnX"),
                PlayerPrefs.GetFloat("RespawnY"),
                0
            );
        }
        else
        {
            RespawnPosition = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "ButlerX")
        {
            RespawnPosition = transform.position;

            PlayerPrefs.SetFloat("RespawnX", RespawnPosition.x);
            PlayerPrefs.SetFloat("RespawnY", RespawnPosition.y);
            PlayerPrefs.Save();

            Debug.Log("Checkpoint updated and saved!");
        }
    }
}
