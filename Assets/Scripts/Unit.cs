using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;


public class Unit : MonoBehaviourPunCallbacks
{
    public int myTeam;
    public GameObject selectionIndicator;
    public Vector3 target;

    [HideInInspector]
    public PhotonView PV;
    [HideInInspector]
    public PlayerManager playerManager;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]

    public bool shouldStop = false;

    public SphereCollider SphereCollider;

    public Animator animator;

    public void Start()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        RoomManager.Instance.allUnitsList.Add(gameObject);
        if (myTeam == 0)
            myTeam = playerManager.team;

        agent.updateRotation = false;
        SphereCollider = GetComponent<SphereCollider>();
    }

    public void UpdateDestination(Vector3 newTarget)
    {
        target = newTarget;
        if(agent.isStopped)
        {
            agent.isStopped = false;
            shouldStop = false;
        }
        agent.SetDestination(target);
    }
    public void Update()
    {
        if (agent.hasPath)
        {
            Vector3 direction = agent.desiredVelocity.normalized;
            if (direction.sqrMagnitude > 0f)
            {
                transform.forward = direction;
            }
        }
    }

    public void OnDestroy()
    {
        RoomManager.Instance.allUnitsList.Remove(gameObject);
    }
}
