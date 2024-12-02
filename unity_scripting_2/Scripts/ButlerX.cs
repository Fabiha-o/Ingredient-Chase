using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class ButlerX : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpforce;
    public float detectionRadius = 5.0f; // Radius to detect nearby ingredients
    private GameObject attachedIngredient = null; // The ingredient currently attached to the player
    public LayerMask groundLayer; // Layer for ground objects

    public struct PlayerStats
    {
        public int health;
        public float speed;

        public PlayerStats(int health, float speed)
        {
            this.health = health;
            this.speed = speed;
        }
    }

    public PlayerStats playerStats = new PlayerStats(100, 5.0f);
    private Vector3 startPosition;
    private List<Vector3> ingredientStartPositions = new List<Vector3>();
    private List<Vector3> enemyStartPositions = new List<Vector3>();

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping
    }

    public PlayerState currentState;
    private Dictionary<string, int> collectedIngredients = new Dictionary<string, int>();
    public float enemyProximityRadius = 4f;

    // Shield ability variables
    public float shieldDuration = 2f;
    public float shieldCooldown = 5f;
    private bool isShieldActive = false;
    private bool canUseShield = true;

    // Event to notify when an ingredient is collected
    public UnityEvent OnIngredientCollected;

    // Portal reference and activation conditions
    public GameObject portal; // The portal GameObject
    public int requiredIngredientsToActivate = 3; // Number of ingredients required to activate the portal
    private bool portalActivated = false; // Whether the portal has been activated

    private void OnEnable()
    {
        Debug.Log("ButlerX enabled. Initializing positions and resetting stats...");
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        StoreInitialPositions();
        playerStats = new PlayerStats(100, 5.0f);
        currentState = PlayerState.Idle;

        // Initialize the UnityEvent if it's not already initialized
        if (OnIngredientCollected == null)
            OnIngredientCollected = new UnityEvent();

        // Subscribe to the event (ensure you have a method to handle it)
        OnIngredientCollected.AddListener(HandleIngredientCollected);

        LoadProgress(); // Load progress when the script is enabled
    }

    private void OnDisable()
    {
        Debug.Log("ButlerX disabled. Cleaning up references...");
        attachedIngredient = null;
        collectedIngredients.Clear();

        // Unsubscribe from the event when disabled
        OnIngredientCollected.RemoveListener(HandleIngredientCollected);

        SaveProgress(); // Save progress when the script is disabled
    }

    void Update()
    {
        if (playerStats.health > 0)
        {
            HandlePlayerMovement();
            CheckEnemyProximity();
        }
        else
        {
            Respawn();
        }

        GameObject nearestIngredient = GetNearestIngredient(detectionRadius);
        if (nearestIngredient != null && Input.GetKeyDown(KeyCode.K))
        {
            AttachIngredientToBack(nearestIngredient);
        }

        // Activate shield with "E" key
        if (Input.GetKeyDown(KeyCode.E) && canUseShield)
        {
            StartCoroutine(ActivateShield());
        }
    }

    void LateUpdate()
    {
        Debug.Log($"Player position after update: {transform.position}");
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        rb.linearVelocity = new Vector2(horizontalInput * playerStats.speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentState = PlayerState.Jumping;
            Jump();
        }

        switch (currentState)
        {
            case PlayerState.Idle:
                Debug.Log("Player is idle.");
                break;
            case PlayerState.Running:
                Debug.Log("Player is running.");
                break;
            case PlayerState.Jumping:
                Debug.Log("Player is jumping.");
                break;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            currentState = PlayerState.Idle;
        }
    }

    void Respawn()
    {
        playerStats.health = 100;

        // Use the respawn position from PlayerPrefs
        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY"))
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("RespawnX"),
                PlayerPrefs.GetFloat("RespawnY"),
                0
            );
        }
        else
        {
            // Fallback to startPosition if no checkpoint has been activated
            transform.position = startPosition;
        }

        currentState = PlayerState.Idle;
        ResetObjectPositions("Ingredient", ingredientStartPositions);
        ResetObjectPositions("Enemy", enemyStartPositions);

        Debug.Log($"Player respawned at: {transform.position}");
    }

    void StoreInitialPositions()
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        foreach (var ingredient in ingredients)
        {
            ingredientStartPositions.Add(ingredient.transform.position);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemyStartPositions.Add(enemy.transform.position);
        }
    }

    void ResetObjectPositions(string tag, List<Vector3> startPositions)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].transform.position = startPositions[i];
        }
    }

    GameObject GetNearestIngredient(float radius)
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        GameObject nearestIngredient = null;
        float closestDistance = radius;

        Vector2 playerPosition = transform.position;

        foreach (GameObject ingredient in ingredients)
        {
            float ingredientDistance = Vector3.Distance(playerPosition, ingredient.transform.position);

            if (ingredientDistance <= closestDistance)
            {
                closestDistance = ingredientDistance;
                nearestIngredient = ingredient;

                if (ingredientDistance == 0f)
                {
                    break;
                }
            }
        }

        return nearestIngredient;
    }

    void AttachIngredientToBack(GameObject ingredient)
    {
        string ingredientName = ingredient.name;

        if (collectedIngredients.ContainsKey(ingredientName))
        {
            collectedIngredients[ingredientName]++;
        }
        else
        {
            collectedIngredients[ingredientName] = 1;
        }

        Debug.Log($"Collected {ingredientName}. Total: {collectedIngredients[ingredientName]}");

        attachedIngredient = ingredient;
        ingredient.transform.SetParent(transform);
        ingredient.transform.localPosition = new Vector3(0, -0.5f, 0);
        ingredient.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        // Mark the ingredient as collected
        ingredient.GetComponent<Ingredient>().Collect();

        // Trigger the OnIngredientCollected event
        OnIngredientCollected.Invoke();
    }

    void CheckEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector2 playerPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(playerPosition, enemy.transform.position);

            if (distanceToEnemy <= enemyProximityRadius && !isShieldActive)
            {
                Debug.Log("Player is too close to an enemy! Respawning...");
                Respawn();
                break;
            }
        }
    }

    IEnumerator ActivateShield()
    {
        isShieldActive = true;
        canUseShield = false;

        // Change player color to indicate shield activation
        GetComponent<SpriteRenderer>().color = Color.blue;

        Debug.Log("Shield activated! Player is invincible.");
        yield return new WaitForSeconds(shieldDuration);

        isShieldActive = false;

        // Revert player color
        GetComponent<SpriteRenderer>().color = Color.white;

        Debug.Log("Shield deactivated.");
        yield return new WaitForSeconds(shieldCooldown);

        canUseShield = true;
    }

    void HandleIngredientCollected()
    {
        Debug.Log("Ingredient collected! Checking if portal can be activated...");

        // Check if the player has collected enough ingredients to activate the portal
        if (collectedIngredients.Count >= requiredIngredientsToActivate && !portalActivated)
        {
            ActivatePortal();
        }
    }

    void ActivatePortal()
    {
        if (!portalActivated)
        {
            Debug.Log("Activating portal...");
            portal.SetActive(true); // Make the portal visible and interactable
            portalActivated = true;
        }
    }

    // Save the player progress (health, position, and collected ingredients)
    void SaveProgress()
    {
        PlayerPrefs.SetInt("PlayerHealth", playerStats.health);
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.SetInt("CollectedIngredients", collectedIngredients.Count);

        // Save the position of the last checkpoint (if applicable)
        PlayerPrefs.SetFloat("RespawnX", transform.position.x);
        PlayerPrefs.SetFloat("RespawnY", transform.position.y);

        PlayerPrefs.Save();
    }

    // Load the player progress (health, position, and collected ingredients)
    void LoadProgress()
    {
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            playerStats.health = PlayerPrefs.GetInt("PlayerHealth");
        }

        if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY"))
        {
            transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), 0);
        }
        
        // Load the number of collected ingredients
        if (PlayerPrefs.HasKey("CollectedIngredients"))
        {
            int ingredientCount = PlayerPrefs.GetInt("CollectedIngredients");
            Debug.Log($"Loaded {ingredientCount} collected ingredients.");
        }
    }
}
