using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour
{
    public string targetSceneName = "LevelThree"; // Name of the scene to load

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player interacts with the door
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door. Loading " + targetSceneName);
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        // Load the specified scene
        SceneManager.LoadScene(targetSceneName);
    }
}
