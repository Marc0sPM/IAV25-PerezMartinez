using UnityEngine;
using UnityEngine.AI;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para el comportamiento de agente que consiste en ser el jugador usando NavMesh
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ControlJugador : ComportamientoAgente
    {
        private NavMeshAgent agenteNavMesh;

        [SerializeField]
        private float offset = 0.1f;

        private LayerMask layerSuelo;

        private void Start()
        {
            agenteNavMesh = GetComponent<NavMeshAgent>();

            //// Puedes configurar algunas propiedades aquí si quieres
            //agenteNavMesh.speed = agente.velocidadMax;
            //agenteNavMesh.acceleration = agente.aceleracionMax;
            //agenteNavMesh.angularSpeed = 720f;

            layerSuelo = LayerMask.GetMask("Suelo");
        }

        private void Update()
        {
            DetectarClic();
        }

        private void DetectarClic()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerSuelo))
                {
                    agenteNavMesh.SetDestination(hit.point);

                    // Opcional: debug ray
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);
                    Debug.DrawRay(hit.point, Vector3.up * 2, Color.green, 2f);
                }
            }
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // Devuelve la dirección actual como vector para mantener compatibilidad
            if (agenteNavMesh.hasPath && agenteNavMesh.remainingDistance > offset)
            {
                direccion.lineal = agenteNavMesh.desiredVelocity;
            }
            else
            {
                direccion.lineal = Vector3.zero;
            }

            return direccion;
        }
    }
}
