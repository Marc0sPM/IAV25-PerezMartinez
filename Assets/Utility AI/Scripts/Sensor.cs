using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UtilityAI
{
    [RequireComponent(typeof(SphereCollider))]
    public class Sensor : MonoBehaviour
    {
        public float radius = 5f;
        public List<string> targetTags = new();
        /// <summary>
        /// The list of detected objects. This is a list of transforms, so you can use it to get the position of the detected objects.
        /// If needed, you can resize the pool.
        /// </summary>
        public List<Transform> detectedObjects = new(25);
        SphereCollider sphereCollider;

        private void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var collider in colliders)
            {
                ProcessTrigger(collider, transform => detectedObjects.Add(transform));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ProcessTrigger(other, transform => detectedObjects.Add(transform));
        }

        private void OnTriggerExit(Collider other)
        {
            ProcessTrigger(other, transform => detectedObjects.Remove(transform));
        }

        void ProcessTrigger(Collider other, Action<Transform> action)
        {
            if (other.CompareTag("Untagged")) return;

            foreach (string tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    action(other.transform);
                }
            }
        }

        public Transform GetClosestTarget(string tag)
        {
            if(detectedObjects.Count == 0)return null;

            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (Transform potentialTarget in detectedObjects)
            {
                if (potentialTarget.CompareTag(tag))
                {
                   Vector3 directionToTarget = potentialTarget.position - position;
                    float distanceToTarget = directionToTarget.sqrMagnitude;
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = potentialTarget;
                    }
                }
            }
            return closestTarget;
        }
    }
}
