using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class ButlerX : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpforce;
    public float detectionRadius = 5.0f;
    private GameObject attachedIngredient = null;
    public LayerMask groundLayer;

    public PlayerStatsSO playerStats;

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

    public float shieldDuration = 2f;
    public float shieldCooldown = 5f;
    private bool isShieldActive = false;
    private bool canUseShield = true;

    public UnityEvent OnIngredientCollected;

    public GameObject portal;
    public int requiredIngredientsToActivate = 3;
    private bool portalActivated = false;

    private void OnEnable()
    {
        Debug.Log("ButlerX enabled.");
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        StoreInitialPositions();

        if (playerStats != null)
        {
            playerStats.health = 100;
            playerStats.speed = 5.0f;
        }

        currentState = PlayerState.Idle;

        if (OnIngredientCollected == null)
            OnIngredientCollected = new UnityEvent();

        OnIngredientCollected.AddListener(HandleIngredientCollected);

        LoadProgress(); // Load player progress (health, position, collected items)
    }

    private void OnDisable()
    {
        Debug.Log("ButlerX disabled.");
        attachedIngredient = null;
        collectedIngredients.Clear();

        OnIngredientCollected.RemoveListener(HandleIngredientCollected);

        SaveProgress(); // Save player progress (health, position, collected items)
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

        if (Input.GetKeyDown(KeyCode.E) && canUseShield)
        {
            StartCoroutine(ActivateShield());
        }
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

        rb.linearVelocity = new Vector2(horizontalInput * playerStats.speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentState = PlayerState.Jumping;
            Jump();
        }

        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Running:
                break;
            case PlayerState.Jumping:
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
        if (playerStats != null)
        {
            playerStats.health = 100;
        }

        // Load the saved checkpoint position
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
            transform.position = startPosition; // Default start position if no checkpoint is set
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

        Rigidbody2D ingredientRb = ingredient.GetComponent<Rigidbody2D>();
        Collider2D ingredientCollider = ingredient.GetComponent<Collider2D>();

        if (ingredientRb != null)
        {
            ingredientRb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (ingredientCollider != null)
        {
            ingredientCollider.enabled = false;
        }

        Ingredient ingredientScript = ingredient.GetComponent<Ingredient>();
        if (ingredientScript != null)
        {
            ingredientScript.Collect();
        }

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

        GetComponent<SpriteRenderer>().color = Color.blue;

        Debug.Log("Shield activated!");
        yield return new WaitForSeconds(shieldDuration);

        isShieldActive = false;

        GetComponent<SpriteRenderer>().color = Color.white;

        Debug.Log("Shield deactivated.");
        yield return new WaitForSeconds(shieldCooldown);

        canUseShield = true;
    }

    void HandleIngredientCollected()
    {
        Debug.Log("Ingredient collected! Checking if portal can be activated...");
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
            portal.SetActive(true);
            portalActivated = true;
        }
    }

    void SaveProgress()
    {
        Debug.Log("Saving player progress...");
        PlayerPrefs.SetInt("Health", playerStats.health);
        PlayerPrefs.SetInt("IngredientsCount", collectedIngredients.Count);

        foreach (var ingredient in collectedIngredients)
        {
            PlayerPrefs.SetInt(ingredient.Key, ingredient.Value);
        }

        // Save checkpoint position
        PlayerPrefs.SetFloat("RespawnX", transform.position.x);
        PlayerPrefs.SetFloat("RespawnY", transform.position.y);
        PlayerPrefs.SetInt("PortalActivated", portalActivated ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        Debug.Log("Loading player progress...");
        if (PlayerPrefs.HasKey("Health"))
        {
            playerStats.health = PlayerPrefs.GetInt("Health");
        }

        if (PlayerPrefs.HasKey("IngredientsCount"))
        {
            int count = PlayerPrefs.GetInt("IngredientsCount");
            collectedIngredients.Clear();
            for (int i = 0; i < count; i++)
            {
                string ingredientKey = "Ingredient_" + i;
                if (PlayerPrefs.HasKey(ingredientKey))
                {
                    collectedIngredients[ingredientKey] = PlayerPrefs.GetInt(ingredientKey);
                }
            }
        }

        if (PlayerPrefs.HasKey("RespawnX"))
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("RespawnX"),
                PlayerPrefs.GetFloat("RespawnY"),
                0
            );
        }

        if (PlayerPrefs.HasKey("PortalActivated"))
        {
            portalActivated = PlayerPrefs.GetInt("PortalActivated") == 1;
            portal.SetActive(portalActivated);
        }
    }
}
