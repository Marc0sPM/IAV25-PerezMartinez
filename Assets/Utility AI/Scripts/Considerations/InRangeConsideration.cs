using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/InRangeConsideration")]
    public class InRangeConsideration : Consideration
    {
        public float maxDistance = 10f;
        public float masAngle = 360f;
        public string targetTag = "Target";
        public AnimationCurve curve;

        public override float Evaluate(Context context)
        {
            if (!context.sensor.targetTags.Contains(targetTag))
            {
                context.sensor.targetTags.Add(targetTag);
            }

            Transform targetTransform = context.sensor.GetClosestTarget(targetTag);
            if (targetTransform == null) return 0f;

            Transform agentTransform = context.agent.transform;
            bool isInRange = agentTransform.InRangeOf(targetTransform, maxDistance, masAngle);
            if (!isInRange) return 0f;

            // Lanzamos un raycast desde el agente hacia el objetivo
            Vector3 origin = agentTransform.position + Vector3.up * 0.5f; // Ajuste de altura si necesario
            Vector3 direction = (targetTransform.position - agentTransform.position).normalized;
            float distance = Vector3.Distance(agentTransform.position, targetTransform.position);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    // Hay una pared bloqueando la vista
                    return 0f;
                }

                // Opcional: asegurarse de que golpeamos el objetivo o que no hay nada más en medio
                if (hit.collider.transform != targetTransform && hit.collider.CompareTag(targetTag) == false)
                {
                    return 0f;
                }
            }

            // Calculamos utilidad en base a la distancia
            Vector3 directionToTarget = targetTransform.position - agentTransform.position;
            float distanceToTarget = directionToTarget.With(y: 0).magnitude;
            float normalizedDistance = Mathf.Clamp01(distanceToTarget / maxDistance);
            float utility = curve.Evaluate(normalizedDistance);
            return Mathf.Clamp01(utility);
        }

        private void Reset()
        {
            curve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }
}
