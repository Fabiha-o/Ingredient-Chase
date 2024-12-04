using System.Collections;
using UnityEngine;

public class Ella : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpForce;
    public float moveSpeed = 5.0f;
    public float detectionRadius = 5.0f; // For checking for ingredients (if required later)

    public Vector3 currentCheckpointPosition;

    private Vector3 startPosition; // Starting position of the player

    public LayerMask groundLayer; // Layer for ground detection

    public float enemyProximityRadius = 4f; // Radius to check enemy proximity

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        // Load the checkpoint position if it exists
        currentCheckpointPosition = LoadCheckpoint();

        // If no saved checkpoint exists, use the starting position
        if (currentCheckpointPosition == Vector3.zero)
        {
            currentCheckpointPosition = startPosition;
        }

        transform.position = currentCheckpointPosition;  // Set the initial position to the checkpoint
    }

    private void Update()
    {
        // Handle movement input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
        {
            Jump();
        }

        // Check for enemy proximity
        CheckEnemyProximity();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset any existing vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        // Use a simple check to see if the player is on the ground
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
    }

    private void CheckEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= enemyProximityRadius)
            {
                // Handle proximity logic, such as respawning or taking damage
                Debug.Log("Player is too close to an enemy!");
            }
        }
    }

    // Set the current checkpoint when the player reaches it
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpointPosition = checkpointPosition;
        SaveCheckpoint(checkpointPosition);  // Save the checkpoint position
        Debug.Log("Checkpoint set at: " + checkpointPosition);
    }

    // Respawn the player at the current checkpoint
    public void RespawnAtCheckpoint()
    {
        transform.position = currentCheckpointPosition;
        Debug.Log("Respawned at checkpoint: " + currentCheckpointPosition);
    }

    // Save the checkpoint position to PlayerPrefs
    private void SaveCheckpoint(Vector3 checkpointPosition)
    {
        PlayerPrefs.SetFloat("EllaCheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat("EllaCheckpointY", checkpointPosition.y);
        PlayerPrefs.SetFloat("EllaCheckpointZ", checkpointPosition.z);
        PlayerPrefs.Save();

        Debug.Log("Ella's checkpoint saved at: " + checkpointPosition);
    }

    // Load the checkpoint position from PlayerPrefs
    private Vector3 LoadCheckpoint()
    {
        if (PlayerPrefs.HasKey("EllaCheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("EllaCheckpointX");
            float y = PlayerPrefs.GetFloat("EllaCheckpointY");
            float z = PlayerPrefs.GetFloat("EllaCheckpointZ");

            Vector3 checkpointPosition = new Vector3(x, y, z);
            Debug.Log("Ella's checkpoint loaded: " + checkpointPosition);
            return checkpointPosition;
        }
        else
        {
            // If no checkpoint is saved, return Vector3.zero (default start position)
            Debug.Log("No saved checkpoint found. Using default start position.");
            return Vector3.zero;
        }
    }
}
