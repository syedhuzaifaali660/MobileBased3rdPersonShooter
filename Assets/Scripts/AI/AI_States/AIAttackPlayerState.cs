
public class AIAttackPlayerState : AIState
{
    public AIStateId GetId() {
        return AIStateId.AttackPlayer;
    }

    public void Enter(AIAgent agent) {
        agent.weapons.ActivateWeapon();
        agent.weapons.SetTarget(agent.playerTransform);
        agent.navMeshAgent.stoppingDistance = 5.0f;
        agent.weapons.SetFiring(true);
    }

    public void Update(AIAgent agent) {
            agent.navMeshAgent.destination = agent.playerTransform.position;
        if (agent.playerTransform.GetComponent<Health>().IsDead())
        {
            agent.stateMachine.ChangeState(AIStateId.Idle);
        }

    }

    public void Exit(AIAgent agent) {
        agent.navMeshAgent.stoppingDistance = 0.0f;
    }


}
