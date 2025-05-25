using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/ForceComparison")]
    public class ForceComparison : Consideration
    {
        public override float Evaluate(Context context)
        {
            float myForce = context.GetData<float>("force");
            float targetForce = 0f;

            if (context.target != null)
            {
                var targetForceComp = context.target.gameObject.GetComponent<Force>();
                if (targetForceComp != null)
                    targetForce = targetForceComp.normalizedForce;
            }

            if (targetForce <= 0f) return 1f; // Avoid division by zero

            float ratio = myForce / targetForce;

            // Example: if ratio >= 1 -> I have more force -> value close to 1
            // if ratio < 1 -> I am weaker -> lower value

           
            return Mathf.Clamp01(ratio);
        }
    }
}
