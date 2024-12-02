using UnityEngine;
using UnityEngine.AI;

public class CountessChaseIngredient : MonoBehaviour
{
    public float movementSpeed = 3f;   // Speed of the Countess
    public float stopDistance = 2f; // Stop 1 inch (0.0254 meters) from the ingredient
    private GameObject targetIngredient; // Target ingredient
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = movementSpeed;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        FindClosestIngredient();

        if (targetIngredient != null)
        {
            MoveTowardIngredient();
        }
    }

    void FindClosestIngredient()
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");

        if (ingredients.Length == 0)
        {
            targetIngredient = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        GameObject closestIngredient = null;

        foreach (GameObject ingredient in ingredients)
        {
            float distanceToIngredient = Vector3.Distance(transform.position, ingredient.transform.position);

            if (distanceToIngredient < closestDistance)
            {
                closestDistance = distanceToIngredient;
                closestIngredient = ingredient;
            }
        }

        targetIngredient = closestIngredient;
    }

    void MoveTowardIngredient()
    {
        if (agent != null)
        {
            // Use NavMeshAgent to move toward the ingredient
            agent.SetDestination(targetIngredient.transform.position);

            // Stop moving if within stopping distance (1 inch)
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
        else
        {
            // Fallback movement logic without NavMeshAgent
            float distanceToTarget = Vector3.Distance(transform.position, targetIngredient.transform.position);

            if (distanceToTarget > stopDistance)
            {
                Vector3 direction = (targetIngredient.transform.position - transform.position).normalized;
                transform.position += direction * movementSpeed * Time.deltaTime;
            }
        }
    }
}
