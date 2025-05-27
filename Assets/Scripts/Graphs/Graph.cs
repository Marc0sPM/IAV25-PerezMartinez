/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        // Aquí el grafo entero es representado con estas listas, que luego puede aprovechar el algoritmo A*.
        // El pseudocódigo de Millington no asume que tengamos toda la información del grafo representada y por eso va guardando registros de los nodos que visita.
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices;
        protected int numCols, numRows;
        

        // this is for informed search like A*
        // Un delegado especifica la cabecera de una función, la que sea, que cumpla con esos parámetros y devuelva ese tipo.
        // Cuidado al implementarlas, porque no puede ser que la distancia -por ejemplo- entre dos casillas tenga una heurística más cara que el coste real de navegar de una a otra.
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;


        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++) {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }


        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // 1. Obtener los vértices de inicio (start) y destino (goal)
            Vertex start = GetNearestVertex(srcO.transform.position);
            Vertex goal = GetNearestVertex(dstO.transform.position);

            if (start == goal) return new List<Vertex>();

            if (start == null || goal == null)
                return new List<Vertex>();

            // 2. Inicializar estructuras necesarias
            Queue<int> openSet = new Queue<int>(); // Usamos una cola para BFS
            bool[] visited = new bool[vertices.Count]; // Para marcar los nodos visitados
            int[] prevList = new int[vertices.Count]; // Para reconstruir el camino

            for (int i = 0; i < vertices.Count; i++)
            {
                visited[i] = false;
                prevList[i] = -1;
            }

            // 3. Inicializar el nodo inicial
            openSet.Enqueue(start.id);
            visited[start.id] = true;

            // 4. Bucle principal de BFS
            while (openSet.Count > 0)
            {
                int current = openSet.Dequeue();

                // 4.1 Comprobar si hemos llegado al objetivo
                if (current == goal.id)
                    return BuildPath(start.id, goal.id, ref prevList);

                // 4.2 Revisar los vecinos del vértice actual
                Vertex[] vecinos = GetNeighbours(vertices[current]);

                for (int j = 0; j < vecinos.Length; j++)
                {
                    int neighborId = vecinos[j].id;

                    // Si el vecino ya ha sido visitado, lo ignoramos
                    if (visited[neighborId])
                        continue;

                    // Marcar el vecino como visitado y agregarlo a la cola
                    visited[neighborId] = true;
                    openSet.Enqueue(neighborId);
                    prevList[neighborId] = current;
                }
            }

            // 5. Si se vacía openSet y no se alcanza goal, no hay camino posible
            return new List<Vertex>();
        }


        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
            {
                // 1. Obtener los vértices de inicio (start) y destino (goal)
                Vertex start = GetNearestVertex(srcO.transform.position);
                Vertex goal = GetNearestVertex(dstO.transform.position);

                if(start == goal) return new List<Vertex>();

                if (start == null || goal == null)
                    return new List<Vertex>();

                // 2. Inicializar estructuras necesarias
                int n = vertices.Count;
                float[] gScore = new float[n];
                float[] fScore = new float[n];
                int[] prevList = new int[n];
                bool[] closedSet = new bool[n];
                List<int> openSet = new List<int>();

                for (int i = 0; i < n; i++)
                {
                    gScore[i] = Mathf.Infinity;
                    fScore[i] = Mathf.Infinity;
                    prevList[i] = -1;
                    closedSet[i] = false;
                }

                // 2.1 Configurar el nodo inicial
                gScore[start.id] = 0;
                fScore[start.id] = h != null ? h(start, goal) : 0;
                openSet.Add(start.id);

                // 3. Bucle principal de A*
                while (openSet.Count > 0)
                {
                    // 3.1 Encontrar el nodo 'current' con menor fScore en openSet
                    int current = openSet[0];
                    for (int i = 1; i < openSet.Count; i++)
                    {
                        if (fScore[openSet[i]] < fScore[current])
                            current = openSet[i];
                    }

                    // 3.2 Comprobar si hemos llegado al objetivo
                    if (current == goal.id)
                        return BuildPath(start.id, goal.id, ref prevList);

                    // 3.3 Mover 'current' de openSet a closedSet
                    openSet.Remove(current);
                    closedSet[current] = true;

                    // 3.4 Revisar los vecinos del vértice actual
                    Vertex[] vecinos = GetNeighbours(vertices[current]);
                    float[] costes = GetNeighboursCosts(vertices[current]);

                    for (int j = 0; j < vecinos.Length; j++)
                    {
                        int neighborId = vecinos[j].id;

                        // Si el vecino ya está en closedSet, lo ignoramos
                        if (closedSet[neighborId])
                            continue;

                        // Calculamos el coste tentativo para llegar al vecino
                        float tentative_g = gScore[current] + costes[j];

                        // Si el vecino no está en openSet, lo añadimos
                        if (!openSet.Contains(neighborId))
                            openSet.Add(neighborId);
                        // Si el camino no mejora, lo ignoramos
                        else if (tentative_g >= gScore[neighborId])
                            continue;

                        // 3.5 Actualizar la mejor ruta encontrada hasta ahora
                        prevList[neighborId] = current;
                        gScore[neighborId] = tentative_g;
                        fScore[neighborId] = tentative_g + (h != null ? h(vecinos[j], goal) : 0);
                    }
                }

                // 4. Si se vacía openSet y no se alcanza goal, no hay camino posible
                return new List<Vertex>();
            }


        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            // Si la ruta tiene 2 o menos nodos, no se puede suavizar
            if (inputPath.Count <= 2)
                return new List<Vertex>(inputPath);

            List<Vertex> outputPath = new List<Vertex>
            {
                inputPath[0] // Agregar el primer nodo
            };

            int inputIndex = 2; // Empezamos en el tercer nodo (índice 2)

            while (inputIndex < inputPath.Count - 1)
            {
                Vertex fromPt = outputPath[outputPath.Count - 1];
                Vertex toPt = inputPath[inputIndex];

                if (!RayClear(fromPt, toPt))
                {
                    // Si hay una colisión, agregar el último nodo seguro
                    outputPath.Add(inputPath[inputIndex - 1]);
                }

                inputIndex++;
            }

            // Agregar el último nodo de la ruta
            outputPath.Add(inputPath[inputPath.Count - 1]);

            return outputPath;
        }

        bool RayClear(Vertex from, Vertex to)
        {
            RaycastHit hit;
            // Asegurarse de que la dirección esté normalizada
            Vector3 direction = (to.transform.position - from.transform.position).normalized;
            float distance = Vector3.Distance(from.transform.position, to.transform.position);

            // Crear un LayerMask para ignorar la capa del suelo
            int layerMask = ~LayerMask.GetMask("Suelo"); // Ignorar la capa "Suelo"

            // Si el rayo no está alineado correctamente, muestra el rayo en la escena
            //Color rayColor = Physics.Raycast(from.transform.position, direction, out hit, distance, layerMask) ? Color.red : Color.green;
            //Debug.DrawRay(from.transform.position, direction * distance, rayColor, 1f);

            // Raycast usando la dirección normalizada
            if (Physics.Raycast(from.transform.position, direction, out hit, distance, layerMask))
            {
                // Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // Depura qué objeto está siendo golpeado
                return false; // Si hay un obstáculo (no es el suelo), retorna false
            }

            return true; // No hay obstáculos, el camino es recto
        }
        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count) 
                return path;

            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

    }
}
