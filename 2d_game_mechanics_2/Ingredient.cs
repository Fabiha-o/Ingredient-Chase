using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public bool isCollected = false;  // Tracks whether the ingredient is collected

    public void Collect()
    {
        isCollected = true;
    }
}
