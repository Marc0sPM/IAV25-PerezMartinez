/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{

    using UnityEngine;

    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador: ComportamientoAgente
    {
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        /// 

        //float tiempoGiroSuave = 0.1f;
        //float velocidadGiroSuave;

        private Vector3 destino;
        private bool moviendo = false;
        [SerializeField]
        private float offset;
        LayerMask layerSuelo;

        void Start()
        {
            destino = transform.position; // Inicializa el destino en la posición actual
            layerSuelo = LayerMask.GetMask("Suelo");
        }

        private void Update()
        {
            DetectarClic(); // Actualiza el destino al hacer clic
            base.Update();  // Mantiene la actualización normal del comportamiento
        }

        void DetectarClic()
        {
            if (Input.GetMouseButtonDown(0)) // Click izquierdo
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerSuelo))
                {
                    destino = hit.point;
                    destino.y = transform.position.y; // Mantener la altura del personaje
                    moviendo = true;

                    // Dibujar el rayo en la escena
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);
                    Debug.DrawRay(hit.point, Vector3.up * 2, Color.green, 2f);
                }
            }
        }

        public override Direccion GetDireccion()
        {
             Direccion direccion = new Direccion();

            if (moviendo)
            {
                Vector3 direccionMovimiento = destino - transform.position;
                direccionMovimiento.y = 0; // No mover en Y

                float distancia = direccionMovimiento.magnitude;

                if (distancia > offset) // Si está fuera del radio de llegada, moverse
                {
                    direccion.lineal = direccionMovimiento.normalized * agente.aceleracionMax;
                }
                else
                {
                    direccion.lineal = Vector3.zero;
                    moviendo = false; // Detener el movimiento al llegar
                }
            }

            return direccion;
        }
    }
}