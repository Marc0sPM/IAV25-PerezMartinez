using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    // Este script va en el hijo y redirige los eventos al padre
    public class SensorTrigger : MonoBehaviour
    {
        public Sensor parent;

        private void OnTriggerEnter(Collider other)
        {
            parent?.HandleTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            parent?.HandleTriggerExit(other);
        }
    }

    public class Sensor : MonoBehaviour
    {
        public float radius = 5f;
        public List<string> targetTags = new();
        public List<Transform> detectedObjects = new(25);

        private SphereCollider sphereCollider;

        private void Awake()
        {
            // Crear hijo "Sensor" si no existe
            Transform sensorTransform = transform.Find("Sensor");
            if (sensorTransform == null)
            {
                GameObject sensorObj = new GameObject("Sensor");
                sensorObj.transform.SetParent(transform);
                sensorObj.transform.localPosition = Vector3.zero;
                sensorTransform = sensorObj.transform;
            }

            GameObject sensorGO = sensorTransform.gameObject;

            // Añadir collider
            sphereCollider = sensorGO.GetComponent<SphereCollider>();
            if (sphereCollider == null)
                sphereCollider = sensorGO.AddComponent<SphereCollider>();

            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius * 2;

            // Añadir el script redirigidor
            SensorTrigger sensorTrigger = sensorGO.GetComponent<SensorTrigger>();
            if (sensorTrigger == null)
                sensorTrigger = sensorGO.AddComponent<SensorTrigger>();

            sensorTrigger.parent = this;

            // Detección inicial
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius * 2);
            foreach (var collider in colliders)
            {
                ProcessTrigger(collider, t => detectedObjects.Add(t));
            }
        }

        public void HandleTriggerStay(Collider other)
        {
            ProcessTrigger(other, t => {
                if (!detectedObjects.Contains(t))
                    detectedObjects.Add(t);
            });
        }

        public void HandleTriggerExit(Collider other)
        {
            ProcessTrigger(other, t => detectedObjects.Remove(t));
        }

        private void ProcessTrigger(Collider other, Action<Transform> action)
        {
            if (other == null || other.CompareTag("Untagged")) return;

            foreach (string tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    action(other.transform);
                    break;
                }
            }
        }

        public Transform GetClosestTarget(string tag)
        {
            if (detectedObjects.Count == 0) return null;

            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (Transform potentialTarget in detectedObjects)
            {
                if (potentialTarget == null || !potentialTarget.CompareTag(tag)) continue;

                float distance = (potentialTarget.position - position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget;
                }
            }

            return closestTarget;
        }
    }
}
