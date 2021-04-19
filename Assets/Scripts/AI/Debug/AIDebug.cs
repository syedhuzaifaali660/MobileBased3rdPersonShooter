using UnityEngine;
using UnityEngine.AI;

public class AIDebug : MonoBehaviour
{
    public bool velocity;
    public bool desieredVelocity;
    public bool path;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    private void OnDrawGizmos()
    {
        if (velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }
        if (desieredVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }
        if (path)
        {
            Gizmos.color = Color.black;
            var agentPath = agent.path;
            Vector3 prevCornor = transform.position;
            foreach (var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCornor, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevCornor = corner;
            }
        }
    }
}
