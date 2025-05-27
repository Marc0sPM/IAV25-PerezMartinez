using System.Collections.Generic;
using System;
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

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        // Acceso al diccionario en tiempo de ejecución
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public void Add(TKey key, TValue value) => dictionary.Add(key, value);

        public bool Remove(TKey key) => dictionary.Remove(key);

        public void Clear()
        {
            dictionary.Clear();
            keys.Clear();
            values.Clear();
        }

        // SERIALIZACIÓN PARA UNITY

        // Antes de serializar, llenamos las listas keys y values con el contenido del diccionario
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in dictionary)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // Después de deserializar, reconstruimos el diccionario a partir de las listas
        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();
            int count = Math.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                // Evitar claves duplicadas en caso de datos corruptos
                if (!dictionary.ContainsKey(keys[i]))
                    dictionary.Add(keys[i], values[i]);
            }
        }
    }

}
