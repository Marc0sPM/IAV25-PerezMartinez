using UnityEngine;


namespace UnityUtils
{
    public class DrawGizmos : MonoBehaviour
    {
        public float radius = 3f;
        public Color color = Color.red;
        public Vector3 offset = Vector3.zero;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
    }
}
