using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de WANDER a otro agente
    /// </summary>
    public class Merodear : ComportamientoAgente
    {
        [SerializeField]
        float maxTime = 2.0f;

        [SerializeField]
        float minTime = 1.0f;

        float t = 3.0f;
        float actualT = 2.0f;

        Direccion lastDir = new Direccion();

        /**
         * 
         * 
         *      !! MODIFICAR PARA ENCRUCIJADA
         * 
         * 
         ------------------------------------------ */
        public override Direccion GetDireccion()
        {
            if (t >= actualT)
            {
                Direccion direccion = new Direccion();

                Vector2 dir = Random.insideUnitCircle.normalized;

                direccion.lineal = new Vector3(dir.x, 0, dir.y);
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;

                lastDir = direccion;

                actualT = Random.Range(minTime, maxTime);

                t = 0.0f;
                //Debug.LogError("CAMBIO DIRECCION" + "TMax = " + maxTime + " TMin = " + minTime);
            }
            else
            {
                t += Time.deltaTime;
            }
            return lastDir;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer != 7 && collision.gameObject.layer != 8)
            {
                Debug.Log("COLLISIONO CON LAYER: " + collision.gameObject.layer);

                t = 0;

                Vector3 normalColision = collision.contacts[0].normal;

                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);

                // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f);

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 direccionReflejada = Vector3.Reflect(ray.direction, normalColision);

                    //Debug.DrawRay(hit.point, direccionReflejada * 10, Color.green, 2.0f); // Rayo reflejado, color verde, duración 2 segundos

                    lastDir.lineal = direccionReflejada.normalized * agente.aceleracionMax;
                }
            }
        }

    }
}
