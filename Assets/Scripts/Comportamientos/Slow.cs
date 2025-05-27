/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    using UCM.IAV.Movimiento;

    public class Slow : MonoBehaviour
    {
        float vel = 0.0f;
        
        /*
         *  Cuando el jugador (identificado con el ControlJugador) se acerca al trigger del
         *  minotauro, su velocidad máxima en el componente Agente se ve reducida enormemente.
         *  Si logra abandonar el trigger, se restaura su velocidad.
         */

        private void OnTriggerEnter(Collider other)
        {
            ControlJugador animator = other.gameObject.GetComponent<ControlJugador>();
            if(!ReferenceEquals(animator, null))
            {
                Agente agent = other.gameObject.GetComponent<Agente>();
                vel = agent.velocidadMax;
                agent.velocidadMax = 1;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ControlJugador animator = other.gameObject.GetComponent<ControlJugador>();
            if (!ReferenceEquals(animator, null))
            {
                Agente agent = other.gameObject.GetComponent<Agente>();
                agent.velocidadMax = vel;
            }
        }
    }
}
