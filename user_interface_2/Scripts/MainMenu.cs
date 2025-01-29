using UnityEngine;
using UnityEngine.SceneManagement; // Make sure this is included

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game On");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Story()
    {
        SceneManager.LoadScene("Story");
    }
}
