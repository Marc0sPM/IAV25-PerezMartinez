using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/CompositeConsideration")]
    public class CompositeConsideration : Consideration
    {
        public enum OperationType {AVERAGE, MULTIPLY, ADD, SUBTRACT, DIVIDE, MAX, MIN};

        public bool allMustBeNonZero = true;

        public OperationType operation = OperationType.MAX;
        public List<Consideration> considerations = new List<Consideration>();

        public override float Evaluate(Context context)
        {
            if(considerations == null || considerations.Count == 0) return 0f;

            float result = considerations[0].Evaluate(context);
            if (result == 0f && allMustBeNonZero) return 0f;


            // Suggestion: Only 2 considerations per Composite 
            for (int i = 0; i < considerations.Count; i++)
            {
                float value = considerations[i].Evaluate(context);

                if(allMustBeNonZero && value == 0f) return 0f;

                switch (operation)
                {
                    case OperationType.AVERAGE:
                        result = (result + value) / 2;
                        break;
                    case OperationType.ADD:
                        result += value;
                        break;
                    case OperationType.SUBTRACT:
                        result -= value;
                        break;
                    case OperationType.MULTIPLY:
                        result *= value;
                        break;
                    case OperationType.DIVIDE:
                        result = value != 0f ? result / value : result; // Prevent division by zero
                        break;
                    case OperationType.MAX:
                        result = Mathf.Max(result, value);
                        break;
                    case OperationType.MIN:
                        result = Mathf.Min(result, value);
                        break;
                }
            } 
            return Mathf.Clamp01(result);
        }
    }   
}
