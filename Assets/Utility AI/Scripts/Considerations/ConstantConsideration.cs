using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityAI;

namespace UtilityAI
{
    /// <summary>
    /// A constant consideration that always returns the same value.
    /// </summary>
    [CreateAssetMenu(fileName = "ConstantConsideration", menuName = "Utility AI/Considerations/Constant")]

    public class ConstantConsideration : Consideration
    {
        public float value;

        public override float Evaluate(Context context) => value;
    }
}
