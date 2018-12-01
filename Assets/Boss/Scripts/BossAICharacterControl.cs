using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BossController))]
public class BossAICharacterControl : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private BossController character;
    public Transform target;

    // Use this for initialization
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<BossController>();

        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);

        float step = agent.speed * Time.deltaTime;
        Vector3 rotationVector = Vector3.RotateTowards(transform.position, target.position, 10f, 0.0f);

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, rotationVector, false);
        }
        else
        {
            character.Move(Vector3.zero, rotationVector, true);
            transform.LookAt(target);
        }

        agent.updatePosition = (agent.remainingDistance > agent.stoppingDistance) && !(character.getIsBusy());
    }
}
