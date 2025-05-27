/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class Llegada : ComportamientoAgente
    {
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        /// 


        // El radio para llegar al objetivo
        public float radioObjetivo;

        // El radio en el que se empieza a ralentizarse
        public float radioRalentizado;

        public float fuerzaRalentizado;

        public float avoidQuantity = 5;
        public int distance = 7;

        // El tiempo en el que conseguir la aceleracion objetivo
        float timeToTarget = 0.1f;
        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // Distancia de objeto al agente
            float distance = (objetivo.transform.position - transform.position).magnitude;

            // Si ha alcanzado el radio objetivo se para
            if (distance < radioObjetivo)
            {
                direccion.lineal = new Vector3(0, 0, 0);
                return direccion;
            }

            float targetAccel;

            // Máxima aceleración desde fuera del radio de frenado
            if (distance > radioRalentizado)
                targetAccel = agente.aceleracionMax;
            // Aceleración escalada
            else
                targetAccel = agente.aceleracionMax * distance / (radioRalentizado * fuerzaRalentizado);

            // Velocity combina aceleración y dirección
            Vector3 targetVelocity = objetivo.transform.position - transform.position;
            targetVelocity.Normalize();
            targetVelocity *= targetAccel;

            // La aceleración se posiciona al nivel de la del objetivo
            direccion.lineal = targetVelocity - agente.velocidad;
            direccion.lineal /= timeToTarget;

            //direccion.lineal += Avoidance();

            // Comprobamos que no se pase de aceleración
            if (direccion.lineal.magnitude > agente.aceleracionMax)
            {
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }


            return direccion;
        }

        Vector3 RayCastCollision(Vector3 pos, Vector3 dir, LayerMask lMask)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, dir, out hit, distance, lMask))
            {
                // Find the line from the gun to the point that was clicked.
                Vector3 incomingVec = hit.point - pos;

                if (incomingVec.magnitude > distance) return Vector3.zero;

                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                // Draw lines to show the incoming "beam" and the reflection.
                //Debug.DrawLine(pos, hit.point, Color.red);
                //Debug.DrawRay(hit.point, reflectVec, Color.green);

                return hit.point + hit.normal * avoidQuantity;
            }
            else
            {
                //Debug.DrawLine(pos, dir * distance, Color.yellow);
                return Vector3.zero;
            }
        }

        Vector3 Avoidance()
        {
            LayerMask lMask = 1 << 8;

            Vector3 dirAcc = Vector3.zero;
            dirAcc += RayCastCollision(transform.position, transform.forward, lMask) * 10;
            dirAcc += RayCastCollision(transform.position, (transform.forward * 2 + transform.right).normalized, lMask);
            dirAcc += RayCastCollision(transform.position, (transform.forward * 2 - transform.right).normalized, lMask);

            return dirAcc.normalized * avoidQuantity;
        }
    }
}
