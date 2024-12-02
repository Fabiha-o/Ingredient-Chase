using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public float spawnInterval = 5.0f;  // Time in seconds between spawns
    public Vector2 spawnAreaSize = new Vector2(10f, 5f);  // Size of the spawn area (width x height)
    private GameObject[] ingredients;  // Array to hold all original ingredients in the scene
    private float spawnTimer = 0f;     // Timer to track spawn interval

    void Start()
    {
        // Find all ingredients with the tag "Ingredient" at the start
        ingredients = GameObject.FindGameObjectsWithTag("Ingredient");

        if (ingredients.Length == 0)
        {
            Debug.LogWarning("No ingredients found with the 'Ingredient' tag!");
        }
    }

    void Update()
    {
        // Increment the timer
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn (shift) an ingredient
        if (spawnTimer >= spawnInterval)
        {
            ShiftIngredientPosition();
            spawnTimer = 0f;  // Reset the timer
        }
    }

    void ShiftIngredientPosition()
    {
        if (ingredients.Length == 0) return;

        // Select a random ingredient from the array
        GameObject ingredient = ingredients[Random.Range(0, ingredients.Length)];

        // Check if the ingredient is still active (not collected)
        if (ingredient.GetComponent<Ingredient>().isCollected)
            return;

        // Generate a random position within the spawn area
        Vector2 randomPosition = new Vector2(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );

        // Move the ingredient to the new position
        ingredient.transform.position = randomPosition;
        Debug.Log($"Moved {ingredient.name} to {randomPosition}");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the spawn area in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
