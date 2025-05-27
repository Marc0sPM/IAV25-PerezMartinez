using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace UtilityAI
{
    public class PatrolState
    {
        public Vector3 initialPosition;
        public float maxHeight;
        public float waitTimer = 0f;
        public bool waiting = false;
    }


    [CreateAssetMenu(menuName = "Utility AI/Actions/Patrol Action")]
    public class PatrolAction : AIAction
    {
        public float patrolRadius = 5f;
        public float destinationMargin = 0.5f;
        public float waitTime = 2f;

        // Cada agente tiene su propio estado
        private Dictionary<NavMeshAgent, PatrolState> agentStates = new Dictionary<NavMeshAgent, PatrolState>();

        public override void Execute(Context context)
        {
            NavMeshAgent agent = context.agent;

            // Obtener o crear estado para este agente
            if (!agentStates.TryGetValue(agent, out PatrolState state))
            {
                state = new PatrolState();
                state.initialPosition = agent.transform.position;
                state.maxHeight = agent.transform.position.y;
                agentStates[agent] = state;
            }

            if (!agent.pathPending && agent.remainingDistance <= destinationMargin)
            {
                if (!state.waiting)
                {
                    state.waiting = true;
                    state.waitTimer = waitTime;
                }
                else
                {
                    state.waitTimer -= Time.deltaTime;
                    if (state.waitTimer <= 0f)
                    {
                        Vector3 newPosition = GetRandomPosition(state.initialPosition, state.maxHeight);
                        agent.SetDestination(newPosition);
                        state.waiting = false;
                    }
                }
            }
            else
            {
                state.waiting = false;
            }
        }

        private Vector3 GetRandomPosition(Vector3 center, float maxHeight)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset2D = Random.insideUnitCircle * patrolRadius;
                Vector3 tentativePosition = new Vector3(center.x + offset2D.x, center.y, center.z + offset2D.y);

                if (NavMesh.SamplePosition(tentativePosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    if (hit.position.y <= maxHeight)
                        return hit.position;
                }
            }

            return center;
        }
    }
}
