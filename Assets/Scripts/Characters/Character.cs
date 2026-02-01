using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Die
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;

    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected CharState state;

    public CharState State { get { return state; } }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }
    public void WalkToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
        }

        SetState(CharState.Walk);
    }
    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, transform.forward);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}
