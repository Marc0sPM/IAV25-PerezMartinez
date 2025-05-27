using UnityEngine;


namespace UnityUtils
{
    public class DrawGizmos : MonoBehaviour
    {
        public float radius = 3f;
        public Color color = Color.red;
        public Vector3 offset = Vector3.zero;
        public bool onSelected = true;

        private void OnDrawGizmos()
        {
            if (!onSelected)
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.position + offset, radius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (onSelected)
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.position + offset, radius);
            }
        }
    }
}
