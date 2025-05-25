using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Actions/Flee Action")]
    public class FleeAction : AIAction
    {
        public Transform closestMino;

        public override void Execute(Context context)
        {

            if(closestMino = MinoManager.Instance.GetClosestMino(context.agent.transform))
            {
                Debug.Log("Fleeing from target");
                context.agent.SetDestination(closestMino.position);
            }
            else
            {
                if (context.target != null)
                {
                    Vector3 fleeDirection = (context.agent.transform.position - context.target.position).normalized;
                    float fleeDistance = 10f;
                    Vector3 fleeTarget = context.agent.transform.position + fleeDirection * fleeDistance;

                    if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    {
                        context.agent.SetDestination(hit.position);
                    }
                    else
                    {
                        Debug.LogWarning("Couldn't find valid flee position on NavMesh.");
                    }
                }
            }

        }
    }
}
