using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            context.agent.SetDestination(target.position);

        }
    }
    
}
