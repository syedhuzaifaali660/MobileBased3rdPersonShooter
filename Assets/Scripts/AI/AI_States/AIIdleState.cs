
using UnityEngine;

public class AIIdleState : AIState
{
    public AIStateId GetId()
    {
        return AIStateId.Idle;
    }

    public void Enter(AIAgent agent)
    {
        agent.weapons.DeactivateWeapon();
        agent.navMeshAgent.ResetPath();
    }


    public void Update(AIAgent agent)
    {
        if (agent.playerTransform.GetComponent<Health>().IsDead()) {
            return;
        }

        Vector3 playerDirection = agent.playerTransform.position - agent.transform.position;
        if(playerDirection.magnitude > agent.config.maxSightDistance) {
            return;
        }

        Vector3 agentDirection = agent.transform.forward;
        playerDirection.Normalize();

        float dotproduct = Vector3.Dot(playerDirection, agentDirection);
        if(dotproduct > 0.0f) {
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
        }
    }
    public void Exit(AIAgent agent)
    {
    }
}
