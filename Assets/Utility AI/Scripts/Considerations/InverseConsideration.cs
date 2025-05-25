using UnityEngine;
namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/InvertedConsideration")]
    public class InvertedConsideration : Consideration
    {
        public Consideration original;

        public override float Evaluate(Context context)
        {
            return 1f - original.Evaluate(context);
        }
    }
}
