using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    [CreateAssetMenu(menuName = "Utility AI/Actions/Idle Action")]
    public class IdleAction : AIAction
    {
        public override void Execute(Context context)
        {
            context.agent.SetDestination(context.agent.transform.position);
        }
    }
}
