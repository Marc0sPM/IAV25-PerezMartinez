using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/InRangeRayCastConsideration")]
    public class InRangeConsideration : Consideration
    {
        public float maxDistance = 10f;
        public float maxAngle = 360f;
        public string targetTag = "Target";
        public string obstacleTag = "";
        public bool useRaycast = true;
        public AnimationCurve curve;

        public override float Evaluate(Context context)
        {
            // Ensure the target tag is present in the sensor's targetTags list
            if (!context.sensor.targetTags.Contains(targetTag))
            {
                context.sensor.targetTags.Add(targetTag);
            }

            // Get the closest target with the specified tag
            Transform targetTransform = context.sensor.GetClosestTarget(targetTag);
            if (targetTransform == null) return 0f;
            context.target = targetTransform;

            Transform agentTransform = context.agent.transform;
            // Check if the target is within range and angle
            bool isInRange = agentTransform.InRangeOf(targetTransform, maxDistance, maxAngle);
            if (!isInRange) return 0f;

            if (useRaycast)
            {
                // Cast a ray from the agent to the target to check for obstacles
                Vector3 origin = agentTransform.position + Vector3.up * 0.5f;
                Vector3 direction = (targetTransform.position - agentTransform.position).normalized;
                float distance = Vector3.Distance(agentTransform.position, targetTransform.position);

                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
                {
                    // If the ray hits an obstacle, return 0 utility
                    if (hit.collider.CompareTag(obstacleTag))
                    {
                        return 0f;
                    }

                    // If the ray hits something that is not the target or does not have the target tag, return 0 utility
                    if (hit.collider.transform != targetTransform && !hit.collider.CompareTag(targetTag))
                    {
                        return 0f;
                    }
                }
            }

            // Calculate utility based on the normalized distance to the target
            Vector3 directionToTarget = targetTransform.position - agentTransform.position;
            float distanceToTarget = directionToTarget.With(y: 0).magnitude;
            float normalizedDistance = Mathf.Clamp01(distanceToTarget / maxDistance);
            float utility = curve.Evaluate(normalizedDistance);
            return Mathf.Clamp01(utility);
        }

        private void Reset()
        {
            // Set a default curve: utility is 1 at distance 0, and 0 at max distance
            curve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }
}
