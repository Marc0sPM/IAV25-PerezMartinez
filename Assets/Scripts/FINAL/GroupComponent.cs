using NUnit.Framework;
using System.Net.WebSockets;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

public class GroupComponent : MonoBehaviour
{
    public int g_id = -1; // Group ID (-1 if alone)
    public string groupTag = "Minotaur";
    public float grpRadius = 3f; // Radius to consider minotaurs in the same group

    void Awake()
    {
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = grpRadius;
        sc.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(groupTag)) return;

        GroupComponent oGrp = other.GetComponentInChildren<GroupComponent>();

        if (oGrp == null) Debug.LogError("GroupComponent of " + other.gameObject + " was null.");

        MinoManager.Instance.AssignSameGroup(this.gameObject, other.gameObject);
    }

    // Check if the minotaur is still in a group, if not, remove it from the group
    private void Update()
    {
        if (g_id == -1) return; // Don't belong to a group

        List<GameObject> minos = MinoManager.Instance.minoGroups.ContainsKey(g_id) ? 
            MinoManager.Instance.minoGroups[g_id] : null;

        if(minos == null) return;

        // Only one minotaur in the group, remove the group
        if(minos.Count <= 1)
        {
            MinoManager.Instance.RemoveFromGroup(gameObject, g_id);
            return;
        }

        bool isNearSomeone = false;
        foreach (var  mino in minos)
        {
            if (mino == this.gameObject) continue; 

            float dist = Vector3.Distance(mino.transform.position, transform.position);
            if(dist <= grpRadius * 1.5f)
            {
                isNearSomeone = true;
                break;
            }
        }

        if(!isNearSomeone)
        {
            MinoManager.Instance.RemoveFromGroup(gameObject, g_id);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, grpRadius * 1.5f);
    }

}
