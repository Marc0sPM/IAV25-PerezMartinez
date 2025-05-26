using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

public class GroupComponent : MonoBehaviour
{
    public int g_id = -1; // Group ID (-1 if alone)
    public string groupTag = "Minotaur";
    public float grpRadius = 3f; // Radius to consider minotaurs in the same group.
    public LayerMask minotaurMask; // Define object which consider to taget.
    public LayerMask obstacleMask; // Define obstacles to not take into account if nearby. 

    void Awake()
    {
        GameObject triggerObject = new GameObject("GroupTrigger");
        triggerObject.transform.SetParent(transform);
        triggerObject.transform.localPosition = Vector3.zero;
        triggerObject.tag = groupTag;

        SphereCollider sc = triggerObject.AddComponent<SphereCollider>();
        sc.radius = grpRadius;
        sc.isTrigger = true;

    }

    public void HandleTriggerEnterFromSelf(Collider other)
    {
        if (!other.CompareTag(groupTag)) return;

        GroupComponent oGrp = other.GetComponentInParent<GroupComponent>();
        if (oGrp == null)
        {
            Debug.LogError("GroupComponent of " + other.gameObject + " was null.");
            return;
        }

        MinoManager.Instance.AssignSameGroup(this.gameObject, other.gameObject);
    }

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grpRadius, minotaurMask);
        bool foundNearby = false;

        foreach (var hit in hits)
        {
            GameObject other = hit.gameObject;
            if (other == gameObject || !other.CompareTag(groupTag)) continue;

            GroupComponent otherComp = other.GetComponentInParent<GroupComponent>();
            if (otherComp == null) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            Vector3 dir = (other.transform.position - transform.position).normalized;

            if (!Physics.Raycast(transform.position, dir, dist + 0.1f, obstacleMask))
            {
               // If alone or belong to diferent groups.
                if (g_id == -1 || otherComp.g_id == -1 || g_id != otherComp.g_id)
                {
                    MinoManager.Instance.AssignSameGroup(gameObject, other);
                }

                foundNearby = true;
            }
        }

        if (g_id != -1 && !foundNearby)
        {
            Debug.Log($"{name} no ha encontrado a nadie cercano, g_id = {g_id}, saliendo del grupo.");
            MinoManager.Instance.RemoveFromGroup(gameObject, g_id);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, grpRadius);
    }
}
