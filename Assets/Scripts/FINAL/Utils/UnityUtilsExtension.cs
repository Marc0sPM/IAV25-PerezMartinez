using UnityEngine;
namespace UnityUtils
{
    public static class UnityUtilsExtensions
    {
        /// <summary>
        /// Retrieves a component of type T from the GameObject. If it doesn't exist, it adds the component to the GameObject.
        /// </summary>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Checks if the target is within a specified distance and angle from the agent.
        /// </summary>
        public static bool InRangeOf(this Transform center, Transform target, float maxDistance, float maxAngle)
        {
            Vector3 directionToTarget = target.position - center.position;
            float distance = directionToTarget.magnitude;

            if (distance > maxDistance)
                return false;

            float angle = Vector3.Angle(center.forward, directionToTarget);
            return angle <= maxAngle * 0.5f; // Because the field of view (FOV) is symmetrical
        }

        /// <summary>
        /// Returns a copy of the vector with optional modified components.
        /// </summary>
        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(
                x ?? v.x,
                y ?? v.y,
                z ?? v.z
            );
        }
    }

}
