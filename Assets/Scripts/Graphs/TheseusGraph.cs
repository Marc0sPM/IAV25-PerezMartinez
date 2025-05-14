/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UCM.IAV.Movimiento;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace UCM.IAV.Navegacion
{


    // Posibles algoritmos para buscar caminos en grafos
    // REALMENTE PARA ESTA PRÁCTICA SÓLO SE NECESITA ASTAR, los otros no los usaremos...
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }

    public class TheseusGraph : MonoBehaviour
    {
        [SerializeField]
        protected Graph graph;

        [SerializeField]
        private TesterGraphAlgorithm algorithm;

        [SerializeField]
        private bool smoothPath = false;

        [SerializeField]
        private string vertexTag = "Vertex"; // Etiqueta de un nodo normal

        [SerializeField]
        private string obstacleTag = "Wall"; // Etiqueta de un obstáculo, tipo pared...

        [SerializeField]
        private Color pathColor;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        private bool ariadna;


        bool firstHeuristic = true;
        Camera mainCamera;
        protected GameObject srcObj;
        protected GameObject dstObj;
        protected List<Vertex> path; // La variable con el camino calculado

        protected LineRenderer hilo;
        protected float hiloOffset = 0.2f;

        // Esferas para el renderizado del hilo de ariadna
        protected List<GameObject> esferas = new List<GameObject>();


        public MinoManager minoManager;

        // Despertar inicializando esto
        public virtual void Awake()
        {
            mainCamera = Camera.main;
            srcObj = GameManager.instance.GetPlayer();
            dstObj = null;
            path = new List<Vertex>();
            hilo = GetComponent<LineRenderer>();
            ariadna = false;

            hilo.startWidth = 0.15f;
            hilo.endWidth = 0.15f;
            hilo.positionCount = 0;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (Input.GetMouseButtonDown(1) && !ariadna)
            {
               updateAriadna(true);
            }
            else if (Input.GetMouseButtonDown(1) && ariadna)
            {
               updateAriadna(false);
                LimpiarEsferas();
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                smoothPath = !smoothPath;
                string msg = smoothPath ? "SMOOTH ENABLED" : "SMOOTH DISABLED";
                Debug.Log(msg);
            }

            if (ariadna)
            {
                //Source jugador y destino el nodo final
                if (srcObj == null) srcObj = GameManager.instance.GetPlayer();
                if (dstObj == null) dstObj = GameManager.instance.GetExitNode();
               
                //path = new List<Vertex>();
                switch (algorithm)
                {
                    case TesterGraphAlgorithm.ASTAR:
                        if (firstHeuristic) path = graph.GetPathAstar(srcObj, dstObj, null); // COMO SEGUNDO ARGUMENTO SE DEBERÍA PASAR LA HEURÍSTICA
                        else path = graph.GetPathAstar(srcObj, dstObj, null); // COMO SEGUNDO ARGUMENTO SE DEBERÍA PASAR LA HEURÍSTICA
                        break;
                    default:
                    case TesterGraphAlgorithm.BFS:
                        path = graph.GetPathBFS(srcObj, dstObj);
                        break;
                    case TesterGraphAlgorithm.DFS:
                        path = graph.GetPathDFS(srcObj, dstObj);
                        break;
                }
                if (smoothPath)
                    path = graph.Smooth(path); // Suavizar el camino, una vez calculado

                if (path.Count > 0)
                {
                    //GameManager.instance.SetPlayerNode(path[path.Count - 1].transform);

                    DibujaHilo();
                }
            }
        }
        void AplicarCosteZonaMinotauros(float factor)
        {
            foreach (GameObject mino in minoManager.listMinotaurs)
            {
                graph.UpdateVertexCost(mino.transform.position, factor);
            }
        }
        public virtual Transform GetNextNode()
        {
            if (path.Count > 0)
                return path[path.Count - 1].transform;

            return null;
        }

        // Dibujado de artilugios en el editor
        // OJO, ESTO SÓLO SE PUEDE VER EN LA PESTAÑA DE SCENE DE UNITY
        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            if (!ReferenceEquals(srcObj, null))
            {
                Gizmos.color = Color.green; // Verde es el nodo inicial
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            if (!ReferenceEquals(dstObj, null))
            {
                Gizmos.color = Color.red; // Rojo es el color del nodo de destino
                v = graph.GetNearestVertex(dstObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            int i;
            Gizmos.color = pathColor;
            for (i = 0; i < path.Count; i++)
            {
                v = path[i];
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
                if (smoothPath && i != 0)
                {
                    Vertex prev = path[i - 1];
                    Gizmos.DrawLine(v.transform.position, prev.transform.position);
                }
            }
        }

        // Mostrar el camino calculado
        public void ShowPathVertices(List<Vertex> path, Color color)
        {
            int i;
            for (i = 0; i < path.Count; i++)
            {
                Vertex v = path[i];
                Renderer r = v.GetComponent<Renderer>();
                if (ReferenceEquals(r, null))
                    continue;
                r.material.color = color;
            }
        }

        // Cuantificación, cómo traduce de posiciones del espacio (la pantalla) a nodos
        private GameObject GetNodeFromScreen(Vector3 screenPosition)
        {
            GameObject node = null;
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit h in hits)
            {
                if (!h.collider.CompareTag(vertexTag) && !h.collider.CompareTag(obstacleTag))
                    continue;
                node = h.collider.gameObject;
                break;
            }
            return node;
        }

        // Dibuja el hilo de Ariadna
        public virtual void DibujaHilo()
        {
            // Establecer el número de posiciones del LineRenderer
            hilo.positionCount = path.Count + 1;
            hilo.SetPosition(0, new Vector3(srcObj.transform.position.x, srcObj.transform.position.y + hiloOffset, srcObj.transform.position.z));

            // Eliminar esferas anteriores antes de generar nuevas
            LimpiarEsferas();

            for (int i = path.Count - 1; i >= 0; i--)
            {
                Vector3 vertexPos = new Vector3(path[i].transform.position.x, path[i].transform.position.y + hiloOffset, path[i].transform.position.z);
                hilo.SetPosition(path.Count - i, vertexPos);

                // Crear esfera en cada nodo con el color del hilo
                CrearEsfera(vertexPos, hilo.startColor);
            }
        }

        void CrearEsfera(Vector3 posicion, Color color)
        {
            GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            esfera.transform.position = posicion;
            esfera.transform.localScale = Vector3.one * 0.6f; // Tamaño de la esfera

            // Desactivar el collider para que no afecte al jugador
            Collider collider = esfera.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            // Aplicar el mismo color del LineRenderer
            Renderer rend = esfera.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.color = color;
            }

            esferas.Add(esfera); // Guardar la esfera para limpiarlas después
        }
        void LimpiarEsferas()
        {
            foreach (GameObject esfera in esferas)
            {
                Destroy(esfera);
            }
            esferas.Clear();
        }


        void updateAriadna(bool ar)
        {
            ariadna = ar;
            hilo.enabled = ariadna;

            if (ariadna)
                AplicarCosteZonaMinotauros(5.0f); // Aumenta el coste
            else
                AplicarCosteZonaMinotauros(1.0f); // Restaura el coste normal
        }

        public string ChangeHeuristic()
        {
            // Está preparado para tener 2 heurísticas diferentes
            firstHeuristic = !firstHeuristic;
            return firstHeuristic ? "Primera" : "Segunda";
        }

        public virtual void ResetPath()
        {
            path = null;
        }
        
    }
}
