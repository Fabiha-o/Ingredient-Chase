using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // The starting point of the platform
    public Transform pointB; // The ending point of the platform
    public float speed = 2f; // Speed of the platform
    private Vector3 target;  // Current target position

    void Start()
    {
        // Set the initial target to pointA
        target = pointA.position;
    }

    void Update()
    {
        // Move the platform towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the platform reached the target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Toggle the target between pointA and pointB
            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw lines in the editor for visualization
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
