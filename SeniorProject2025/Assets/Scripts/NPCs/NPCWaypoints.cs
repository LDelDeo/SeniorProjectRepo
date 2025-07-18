using UnityEngine;
using UnityEngine.AI;

public class NPCWaypoints : MonoBehaviour
{
    public GameObject[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].transform.position);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].transform.position);
        }

        Vector3 direction = agent.velocity.normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(0, -100f, 0), Time.deltaTime * 5f);

        }
    }

}
