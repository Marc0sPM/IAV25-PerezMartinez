using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Actions/MoveToTargetAction")]
    public class MoveToTargetAction : AIAction
    {
        public override void Initialize(Context context)
        {
            // Action tag is added to the tags list of the sensor
            context.sensor.targetTags.Add(targetTag);

        }

        public override void Execute(Context context)
        {
          var target = context.sensor.GetClosestTarget(targetTag);
            if (target == null) return;
            NavMeshAgent agent = context.agent; 

            agent.SetDestination(target.position);

            agent.updateRotation = false;
            Vector3 direction = (agent.steeringTarget - agent.transform.position).With(y:0).normalized;

            if(direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

        }
    }
    
}
