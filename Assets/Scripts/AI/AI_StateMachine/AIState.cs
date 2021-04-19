﻿


public enum AIStateId
{
    ChasePlayer,
    Death,
    Idle,
    FindWeapon,
    AttackPlayer
}

public interface AIState 
{
    AIStateId GetId();
    void Enter(AIAgent agent);
    void Update(AIAgent agent);
    void Exit(AIAgent agent);
}
