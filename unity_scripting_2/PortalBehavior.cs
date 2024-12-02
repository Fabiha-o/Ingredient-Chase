using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
using UnityEngine.Events; // Required for UnityEvents

public class PortalBehavior : MonoBehaviour
{
    public string sceneToLoad = "SecondLevel"; // Scene to load
    private bool portalActivated = false; // Check if the portal should appear
    private Vector3 playerStartPosition; // The player's start position to place the portal
    private ButlerX playerScript; // Reference to the player script
    public UnityEvent onPortalActivated; // Unity event to trigger when the portal appears

    private void OnEnable()
    {
        // Store the player's start position
        playerStartPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        // Get reference to the player script
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ButlerX>();

        // Add listener to the event. Use AddListener to add the method to the UnityEvent
        if (playerScript != null && playerScript.OnIngredientCollected != null)
        {
            playerScript.OnIngredientCollected.AddListener(ActivatePortal);
        }
    }

    private void OnDisable()
    {
        // Remove the listener when the portal is disabled
        if (playerScript != null && playerScript.OnIngredientCollected != null)
        {
            playerScript.OnIngredientCollected.RemoveListener(ActivatePortal);
        }
    }

    void Start()
    {
        // Set the portal position to the player's starting position
        transform.position = playerStartPosition;
    }

    void Update()
    {
        // If the portal is activated and the player enters the portal, teleport them
        if (portalActivated && playerScript != null && Vector3.Distance(transform.position, playerScript.transform.position) < 2f)
        {
            TeleportPlayer();
        }
    }

    // Method to activate the portal after ingredient is collected
    void ActivatePortal()
    {
        portalActivated = true;
        onPortalActivated.Invoke(); // Trigger any events attached to the portal activation
        Debug.Log("Portal Activated! Enter to go to the next level.");
    }

    // Method to teleport the player to the next scene
    void TeleportPlayer()
    {
        SceneManager.LoadScene(sceneToLoad); // Load the second level scene
    }

    // Visualize portal in Scene view when selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
