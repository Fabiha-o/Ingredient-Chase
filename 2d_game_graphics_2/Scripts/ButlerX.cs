using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class ButlerX : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpforce = 15f; // Adjust jump force if necessary
    public float detectionRadius = 5.0f;
    private GameObject attachedIngredient = null;
    bool isGrounded=false;
    Animator animator;

    public PlayerStatsSO playerStats;

    private Vector3 startPosition = new Vector3(-12.75f, 0.04f, 0f); // Default starting position
    private Vector3 checkpointPosition; // Position to save progress
    private bool checkpointSet = false;

    private List<Vector3> ingredientStartPositions = new List<Vector3>();
    private List<Vector3> enemyStartPositions = new List<Vector3>();

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Attacking
    }

    public PlayerState currentState;
    private Dictionary<string, int> collectedIngredients = new Dictionary<string, int>();
    public float enemyProximityRadius = 4f;

    public float shieldDuration = 2f;
    public float shieldCooldown = 5f;
    private bool isShieldActive = false;
    private bool canUseShield = true;

    public int requiredIngredientsToActivate = 1; // Number of ingredients required for scene transition

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
        checkpointPosition = startPosition; // Initialize checkpoint position

        StoreInitialPositions();

        if (playerStats != null)
        {
            playerStats.health = 100;
            playerStats.speed = 5.0f;
        }

        currentState = PlayerState.Idle;

        LoadProgress(); // Load saved progress
    }

    private void Update()
    {
        if(!PauseMenu.isPaused)
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
    }
//Player Movements
    private void HandlePlayerMovement()
{
    float horizontalInput = Input.GetAxis("Horizontal");
    rb.linearVelocity = new Vector2(horizontalInput * playerStats.speed, rb.linearVelocity.y);

    
    if (Input.GetKeyDown(KeyCode.UpArrow)&& isGrounded)
    {
        Jump();
        isGrounded=false;
        animator.SetBool("isJumping", !isGrounded);
    }
    else if (horizontalInput != 0)
    {
        currentState = PlayerState.Running;
    }
    else
    {
        currentState = PlayerState.Idle;
    }
}


    private void Jump()
{
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset vertical velocity
    rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
    currentState = PlayerState.Jumping;
}

    private void OnTriggerEnter2D(Collider2D collision){
        isGrounded=true;
        animator.SetBool("isJumping", !isGrounded);
    }

    private void FixedUpdate(){
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
        animator.SetBool("isJumping", !isGrounded);
    }*/

    private void Respawn()
    {
        playerStats.health = 100;

        transform.position = checkpointSet ? checkpointPosition : startPosition;
        currentState = PlayerState.Idle;

        ResetObjectPositions("Ingredient", ingredientStartPositions);
        ResetObjectPositions("Enemy", enemyStartPositions);

        Debug.Log($"Player respawned at: {transform.position}");
    }

    private void StoreInitialPositions()
    {
        foreach (var ingredient in GameObject.FindGameObjectsWithTag("Ingredient"))
        {
            ingredientStartPositions.Add(ingredient.transform.position);
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyStartPositions.Add(enemy.transform.position);
        }
    }

    private void ResetObjectPositions(string tag, List<Vector3> startPositions)
    {
        var objects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < objects.Length; i++)
        {
            if (i < startPositions.Count)
            {
                objects[i].transform.position = startPositions[i];
            }
        }
    }

    private GameObject GetNearestIngredient(float radius)
    {
        var ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        GameObject nearest = null;
        float closestDistance = radius;

        foreach (var ingredient in ingredients)
        {
            float distance = Vector2.Distance(transform.position, ingredient.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = ingredient;
            }
        }

        return nearest;
    }

    private void AttachIngredientToBack(GameObject ingredient)
    {
        string ingredientName = ingredient.name;

        if (!collectedIngredients.ContainsKey(ingredientName))
            collectedIngredients[ingredientName] = 0;

        collectedIngredients[ingredientName]++;
        attachedIngredient = ingredient;

        ingredient.transform.SetParent(transform);
        ingredient.transform.localPosition = new Vector3(0, -0.5f, 0);

        var rb = ingredient.GetComponent<Rigidbody2D>();
        var collider = ingredient.GetComponent<Collider2D>();

        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
        if (collider != null) collider.enabled = false;

        Debug.Log($"Collected ingredient: {ingredientName}");

        CheckSceneTransition();
    }

    private void CheckSceneTransition()
    {
        if (collectedIngredients.Count >= requiredIngredientsToActivate)
        {
            Debug.Log("All ingredients collected! Transitioning to next level...");
            SceneManager.LoadScene("SecondLevel");
        }
    }

    private void CheckEnemyProximity()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= enemyProximityRadius && !isShieldActive)
            {
                Debug.Log("Enemy too close! Respawning...");
                Respawn();
                break;
            }
        }
    }

    private IEnumerator ActivateShield()
    {
        isShieldActive = true;
        canUseShield = false;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(shieldDuration);

        isShieldActive = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(shieldCooldown);
        canUseShield = true;
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            checkpointPosition = new Vector3(
                PlayerPrefs.GetFloat("CheckpointX"),
                PlayerPrefs.GetFloat("CheckpointY"),
                0
            );
            checkpointSet = true;
        }
    }
}
