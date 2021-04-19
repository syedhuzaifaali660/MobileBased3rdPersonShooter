﻿using UnityEngine;
using UnityEngine.AI;

public class AIChasePlayerState : AIState
{


    float timer = 0.0f;
    AIStateId AIState.GetId() {
        return AIStateId.ChasePlayer;
    }

    void AIState.Enter(AIAgent agent) {

    }


    void AIState.Update(AIAgent agent) {
        if (!agent.enabled) {
            return;
        }

        timer -= Time.deltaTime;
        if (!agent.navMeshAgent.hasPath) {
            agent.navMeshAgent.destination = agent.playerTransform.position;
        }

        if (timer < 0.0f)
        {
            Vector3 direction = (agent.playerTransform.position - agent.navMeshAgent.destination);
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.playerTransform.position;
                }
            }
            timer = agent.config.maxTime;
        }

    }
    void AIState.Exit(AIAgent agent)
    {
       
    }
}
