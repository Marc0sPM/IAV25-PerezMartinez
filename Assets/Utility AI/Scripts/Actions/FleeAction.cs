using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Actions/Flee Action")]
    public class FleeAction : AIAction
    {
        public float minDistanceFromTarget = 20f;
        public float maxFleeDistance = 50f;
        public int maxAttempts = 20;
        public float maxAllowedHeightDifference = 2.0f;  // Evita zonas muy elevadas
        public float rotationSpeed = 5f; // Velocidad de rotación suave

        private Vector3? currentFleeDestination = null;

        public override void Execute(Context context)
        {
            NavMeshAgent agent = context.agent;
            agent.updateRotation = false; // Control manual de rotación

            // Obtener el minotauro más cercano
            Transform closestMino = MinoManager.Instance.GetClosestMino(agent.transform);

            if (closestMino != null)
            {
                currentFleeDestination = null;
                agent.SetDestination(closestMino.position);
                RotateAgentTowardsSteeringTarget(agent);
                Debug.Log("Fleeing from minotaur");
                return;
            }

            if (context.target != null)
            {
                Vector3 currentPos = agent.transform.position;

                if (currentFleeDestination.HasValue)
                {
                    float remainingDistance = Vector3.Distance(currentPos, currentFleeDestination.Value);
                    if (!agent.pathPending && remainingDistance <= agent.stoppingDistance + 0.5f)
                    {
                        Debug.Log("Reached flee destination. Resetting.");
                        currentFleeDestination = null;
                    }
                    else
                    {
                        Debug.Log("Continuing to previous flee destination.");
                        agent.SetDestination(currentFleeDestination.Value);
                        RotateAgentTowardsSteeringTarget(agent);
                        return;
                    }
                }

                Vector3 targetPos = context.target.position;
                Vector3 directionToTarget = (targetPos - currentPos).normalized;

                for (int i = 0; i < maxAttempts; i++)
                {
                    Vector2 randomCircle = Random.insideUnitCircle.normalized;
                    Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
                    Vector3 rawDirection = -directionToTarget + randomDirection * 0.5f;
                    Vector3 fleeDirection = rawDirection != Vector3.zero ? rawDirection.normalized : -directionToTarget;

                    Vector3 candidatePos = currentPos + fleeDirection * Random.Range(6f, maxFleeDistance);

                    if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, MinoManager.Instance.graph.mapSize(), NavMesh.AllAreas))
                    {
                        float distToTarget = Vector3.Distance(hit.position, targetPos);
                        float heightDifference = Mathf.Abs(hit.position.y - currentPos.y);

                        if (distToTarget >= minDistanceFromTarget && heightDifference <= maxAllowedHeightDifference)
                        {
                            currentFleeDestination = hit.position;
                            agent.SetDestination(hit.position);
                            RotateAgentTowardsSteeringTarget(agent);
                            Debug.Log("Fleeing randomly to " + hit.position);
                            return;
                        }
                    }
                }

                Debug.LogWarning("Couldn't find a valid random flee position on NavMesh.");
            }
        }

        private void RotateAgentTowardsSteeringTarget(NavMeshAgent agent)
        {
            // Dirección hacia el steering target, ignorando componente Y para rotación en plano horizontal
            Vector3 direction = agent.steeringTarget - agent.transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
