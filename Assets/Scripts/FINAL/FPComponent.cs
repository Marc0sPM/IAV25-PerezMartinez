using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FPComponent : MonoBehaviour
{
    [Header("Visual Settings")]
    public float radius = 0.5f;
    public Color visualColor = Color.yellow;

    [Header("Collection Settings")]
    public LayerMask collectibleLayer;
    public bool destroyOnCollect = true;

    private SphereCollider sphereCollider;
    private MeshRenderer visualRenderer;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        
        sphereCollider.isTrigger = true;
        sphereCollider.radius = radius;

        if (GetComponent<MeshFilter>() == null)
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * radius * 2f;

            visualRenderer = visual.GetComponent<MeshRenderer>();
            visualRenderer.material.color = visualColor;

            Destroy(visual.GetComponent<Collider>()); // Eliminar el collider de la esfera visual
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collectibleLayer) != 0)
        {
            Force force = other.GetComponent<Force>();
            if(!force)
            {
                Debug.LogWarning($"El objeto {other.name} no tiene un componente Force.");
                return;
            }

            force.force++; 

            if (destroyOnCollect)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = visualColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}