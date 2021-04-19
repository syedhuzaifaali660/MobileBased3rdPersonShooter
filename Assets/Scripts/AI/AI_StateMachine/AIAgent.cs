using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public AIStateId initialState;
    public AIAgentConfig config;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public AIWeapons weapons;

    //AI DEATH STATE
    [HideInInspector] public SkinnedMeshRenderer mesh;
    [HideInInspector] public UIHealthBar healthBar;
    [HideInInspector] public Ragdoll ragdoll;
    [HideInInspector] public Rigidbody rb;


    void Start()
    {
        //AI DEATH STATE
        ragdoll = GetComponent<Ragdoll>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        healthBar = GetComponentInChildren<UIHealthBar>();
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        weapons = GetComponent<AIWeapons>();

        stateMachine = new AIStateMachine(this);
        stateMachine.RegisterState(new AIChasePlayerState());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.RegisterState(new AIIdleState());
        stateMachine.RegisterState(new AIFindWeaponState());
        stateMachine.RegisterState(new AIAttackPlayerState());

        stateMachine.ChangeState(initialState);

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        stateMachine.Update();
    }
}
