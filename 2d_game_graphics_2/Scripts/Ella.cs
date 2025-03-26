using System.Collections;
using UnityEngine;

public class Ella : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpForce = 10f;
    public float moveSpeed = 5.0f;

    public Vector3 currentCheckpointPosition;

    private Vector3 startPosition;

    public float enemyProximityRadius = 4f;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        currentCheckpointPosition = LoadCheckpoint();

        if (currentCheckpointPosition == Vector3.zero)
        {
            currentCheckpointPosition = startPosition;
        }

        transform.position = currentCheckpointPosition;
    }

    private void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        CheckEnemyProximity();
    }

    private void HandleMovement()
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

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= enemyProximityRadius)
            {
                Debug.Log("Player is too close to an enemy!");
                RespawnAtCheckpoint();
                break;
            }
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpointPosition = checkpointPosition;
        SaveCheckpoint(checkpointPosition);
        Debug.Log("Checkpoint set at: " + checkpointPosition);
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = currentCheckpointPosition;
        Debug.Log("Respawned at checkpoint: " + currentCheckpointPosition);
    }

    private void SaveCheckpoint(Vector3 checkpointPosition)
    {
        PlayerPrefs.SetFloat("EllaCheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat("EllaCheckpointY", checkpointPosition.y);
        PlayerPrefs.SetFloat("EllaCheckpointZ", checkpointPosition.z);
        PlayerPrefs.Save();

        Debug.Log("Ella's checkpoint saved at: " + checkpointPosition);
    }

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
            Debug.Log("No saved checkpoint found. Using default start position.");
            return Vector3.zero;
        }
    }
}
