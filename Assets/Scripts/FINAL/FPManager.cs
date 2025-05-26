using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UCM.IAV.Navegacion;

public class FPManager : MonoBehaviour
{
    public GameObject fpPrefab;
    public int numFP = 5;
    public float spawnRadius = 3f;
    public float maxHeightAllowed = 3f;
    public float sampleRange = 1f;
    public GraphGrid graph;

    private List<GameObject> spawnedPelotas = new();

    public void GenerateFP()
    {
        for (int i = 0; i < numFP; i++)
        {
            if (TryGetRandomNavMeshPosition(out Vector3 spawnPos))
            {
                GameObject pelota = Instantiate(fpPrefab, spawnPos + Vector3.up * 0.3f, Quaternion.identity);
                spawnedPelotas.Add(pelota);
            }
            else
            {
                Debug.LogWarning("No se pudo colocar una pelota en una posición válida.");
            }
        }
    }

    bool TryGetRandomNavMeshPosition(out Vector3 result)
    {
        float areaRadius = graph.mapSize() / 2f;
        Vector3 navmeshCenter = new Vector3(areaRadius, 0, areaRadius);

        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector3 randomPoint = navmeshCenter + Random.insideUnitSphere * areaRadius * 1.5f;
            randomPoint.y = 0;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleRange, NavMesh.AllAreas))
            {
                if (hit.position.y <= maxHeightAllowed)
                {
                    result = hit.position;
                    return true;
                }
                else
                {
                    Debug.Log($"Posición descartada por altura: {hit.position.y}");
                }
            }
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (graph != null)
        {
            float r = graph.mapSize() / 2f;
            Vector3 center = transform.position + new Vector3(r, 0, r);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, r * 1.5f);
        }
    }
}
