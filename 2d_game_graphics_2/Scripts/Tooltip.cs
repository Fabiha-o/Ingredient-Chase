using UnityEngine;
using TMPro; // Import TextMeshPro for text updates

public class Tooltip : MonoBehaviour
{
    private static Tooltip current;
    public TextMeshProUGUI tooltipText;
    public GameObject tooltipObject;

    private void Awake()
    {
        current = this;
        Hide(); // Start hidden
    }

    public static void Show(string message)
    {
        if (current != null)
        {
            current.tooltipObject.SetActive(true);
            current.tooltipText.text = message;
        }
    }

    public static void Hide()
    {
        if (current != null)
        {
            current.tooltipObject.SetActive(false);
        }
    }
}
