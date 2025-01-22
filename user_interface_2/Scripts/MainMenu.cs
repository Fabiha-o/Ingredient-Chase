using UnityEngine;

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
