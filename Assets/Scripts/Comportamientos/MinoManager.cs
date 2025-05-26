/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using JetBrains.Annotations;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace UCM.IAV.Navegacion
{

    [System.Serializable]
    public class IntListGODictionary : SerializableDictionary<int, List<GameObject>> { }

    public class MinoManager : MonoBehaviour
    {
        bool staticMino = false;

        [Header("Prefabs")]
        public GameObject minotaur;
        public GameObject static_minotaur;

        [Header("Spawning Area Settings")]
        public float spawnRadius = 2f;
        public float sampleRange = 10f;
        private int nextGroupId = 1;
        public float maxHeightAllowed = 1f;
        private float areaRadius;

        public FPManager fpManager; 

        /** ---------------------------------------------------/*
         * 
         *          DEPRECATED !!
         * 
         */
        public GraphGrid graph;
        /*-------------------------------------------------------*/


        NavMeshSurface navMeshSurface; 
        

        public List<GameObject> listMinotaurs = new();

        public IntListGODictionary minoGroups = new();
        
        public Queue<int> recycledGroupIds = new();



        public static MinoManager Instance { get; private set; } 

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StartUp();
        }

        void StartUp()
        {
            GameObject navGO = GameObject.Find("GraphGrid");

            if (navGO != null)
            {
                /** -------------------------------/*
                 * 
                 *      DEPRECATED !!
                 */
                graph = navGO.GetComponent<GraphGrid>();

                /*--------------------------------- */

                navMeshSurface = navGO.GetComponent<NavMeshSurface>();
            }

            navMeshSurface.BuildNavMesh();

            GenerateMinoGroups();
            fpManager.GenerateFP();

            navMeshSurface.BuildNavMesh();


        }

        void GenerateMinoGroups()
        {
            GameManager gm = GameManager.instance;

            for (int i = 0; i < gm.numSpawns; i++)
            {
                if (!TryGetRandomNavMeshPosition(out Vector3 basePos))
                {
                    Debug.LogWarning("Could not find a valid NavMesh position for minotaur spawn.");
                    continue;
                }

                int groupSize = Random.Range(gm.minGroupSize, gm.maxGroupSize + 1);
                List<GameObject> groupMembers = new();

                for (int j = 0; j < groupSize; j++)
                {
                    Vector3 offset = Random.insideUnitCircle.normalized * spawnRadius;
                    Vector3 candidatePos = basePos + new Vector3(offset.x, 0, offset.y);

                    if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                    {
                        float distToBase = Vector3.Distance(hit.position, basePos);

                        if (distToBase <= spawnRadius * 1.2f)
                        {
                            GameObject prefabToUse = gm.staticMino ? (Random.Range(0, 2) == 0 ? minotaur : static_minotaur) : minotaur;
                            GameObject mino = Instantiate(prefabToUse, hit.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                            listMinotaurs.Add(mino);
                            groupMembers.Add(mino);

                            Force force = mino.GetComponent<Force>();
                            if (force != null)
                            {
                                force.force = Random.Range(gm.minForce, gm.maxForce + 1);
                            }
                            else
                            {
                                Debug.LogWarning("Force component not found on minotaur prefab.");
                            }
                        }

                    }

                    if (groupMembers.Count >= 2)
                    {
                        GameObject first = groupMembers[0];
                        GameObject second = groupMembers[1];

                        AddToNewGroup(first, second);

                        for (int k = 2; k < groupMembers.Count; k++)
                        {
                            AddToGroup(groupMembers[k], first.GetComponentInParent<GroupComponent>().g_id);
                        }

                    }
                }
            }

            bool TryGetRandomNavMeshPosition(out Vector3 result)
            {
                areaRadius = graph.mapSize() / 2f;
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

        }
        
        /** --------------------------------------------------/*
         * 
         *      DEPRECATED !! 
         * 
         */
        void GenerateMino()
        {
            if(staticMino)
            {
                int random = Random.Range(0, 1);
                if(random == 0)
                {
                    listMinotaurs.Add(Instantiate(minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity));
                }
                else
                {
                    listMinotaurs.Add(Instantiate(static_minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity));
                }
            }
            // For Utility AI project, is only used one prefab, Minotaur (which has all the config)
            else
            {
                listMinotaurs.Add(Instantiate(minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity));
            }
        }

        /*---------------------------------------------------------*/

        public Transform GetClosestMino(Transform origin)
        {
            if (listMinotaurs.Count == 0) return null;

            Transform closestMino = null;
            float closestDistance = Mathf.Infinity;
            Vector3 position = origin.position;

            GroupComponent originGroup = origin.GetComponentInParent<GroupComponent>();
            int originGid = originGroup != null ? originGroup.g_id : -1;

            foreach (GameObject mino in listMinotaurs)
            {
                if (origin == mino.transform) continue;

                GroupComponent minoGroup = mino.GetComponentInParent<GroupComponent>();
                int minoGid = minoGroup != null ? minoGroup.g_id : -1;

                // Saltar si es del mismo grupo
                if (originGid != -1 && originGid == minoGid)
                    continue;

                float distance = Vector3.Distance(position, mino.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMino = mino.transform;
                }
            }

            return closestMino;
        }

        /// <summary>
        /// Put in the same group two minotaurs. 
        /// </summary>
        public void AssignSameGroup(GameObject mino1, GameObject mino2)
        {
            GroupComponent comp1 = mino1.GetComponentInParent<GroupComponent>();
            GroupComponent comp2 = mino2.GetComponentInParent<GroupComponent>();
            if(comp1 == null || comp2 == null)
            {
                Debug.LogError("Tried to assign to a group a null mino group component");
            }

            // If both minotaurs are alone, create a new group
            if (comp1.g_id == -1 && comp2.g_id == -1)
            {
                AddToNewGroup(mino1, mino2);
            }
            // If mino1 alone, but mino2 has a group, add mino1 to that group
            else if (comp1.g_id == -1 && comp2.g_id > -1)
            {
                AddToGroup(mino1, comp2.g_id);
            }
            // If mino2 alone, but mino1 has a group, add mino2 to that group
            else if (comp1.g_id > -1 && comp2.g_id == -1)
            {
                AddToGroup(mino2, comp1.g_id);
            }
            //If both minotaurs have a group, add which has smaller group to the other one
            else if (comp1.g_id > -1 && comp2.g_id > -1)
            {
                // If both minotaurs are in the same group, do nothing
                if (comp1.g_id == comp2.g_id) return;
                // If mino2 group is smaller than mino1 group, add mino2 to mino1 group
                else if (minoGroups[comp1.g_id].Count > minoGroups[comp2.g_id].Count)
                {
                    RemoveFromGroup(mino2, comp2.g_id);
                    AddToGroup(mino2, comp1.g_id); 
                }
                // If mino1 group is smaller than mino2 group, add mino1 to mino2 group
                else
                {
                    RemoveFromGroup(mino1, comp1.g_id); 
                    AddToGroup(mino1, comp2.g_id);
                }
            }
        }

        public void AddToNewGroup(GameObject mino1, GameObject mino2)
        {
            if (mino1 == null || mino2 == null)
            {
                Debug.LogError("Cannot add null minotaur to group.");
                return;
            }

            // Reutiliza ID reciclado si hay alguno
            int newGroupId = recycledGroupIds.Count > 0 ? recycledGroupIds.Dequeue() : nextGroupId++;

            Debug.Log("Creating new group with ID: " + newGroupId);

            List<GameObject> groupList = new();
            minoGroups[newGroupId] = groupList;

            if (!groupList.Contains(mino1))
            {
                groupList.Add(mino1);
                mino1.GetComponentInChildren<GroupComponent>().g_id = newGroupId;
            }

            if (!groupList.Contains(mino2))
            {
                groupList.Add(mino2);
                mino2.GetComponentInParent<GroupComponent>().g_id = newGroupId;
            }
        }

        public void AddToGroup(GameObject mino, int groupId)
        {
            if (mino == null)
            {
                Debug.LogError("Cannot add null minotaur to group.");
                return;
            }
            if (!minoGroups.ContainsKey(groupId))
            {
                Debug.LogError("Group ID " + groupId + " does not exist.");
                return;
            }
            if (!minoGroups[groupId].Contains(mino))
            {
                minoGroups[groupId].Add(mino);
                mino.GetComponentInParent<GroupComponent>().g_id = groupId;
            }
        }

        public void RemoveFromGroup(GameObject mino, int gId)
        {
            if (mino == null)
            {
                Debug.LogError("Cannot delete null minotaur from group.");
                return;
            }
            if (!minoGroups.ContainsKey(gId))
            {
                Debug.LogError("Group ID " + gId + " does not exist.");
                return;
            }

            if (minoGroups[gId].Contains(mino))
            {
                minoGroups[gId].Remove(mino);
                mino.GetComponentInParent<GroupComponent>().g_id = -1;

                if (minoGroups[gId].Count  <= 1)
                {
                    foreach (var minoGO in minoGroups[gId])
                    {
                        minoGO.GetComponentInParent<GroupComponent>().g_id = -1;
                    }
                    minoGroups.Remove(gId);
                    recycledGroupIds.Enqueue(gId);
                   //  Debug.Log("Group " + gId + " deleted and ID recycled.");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(areaRadius, 0, areaRadius), areaRadius * 1.5f);
        }
    }
}
