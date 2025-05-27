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

namespace UCM.IAV.Movimiento
{
    public class Teseo : MonoBehaviour
    {

        bool ariadna = false;

        SeguirCamino segCam;
        ControlJugador contJug;

        // Start is called before the first frame update
        void Start()
        {
            segCam = GetComponent<SeguirCamino>();
            contJug = GetComponent<ControlJugador>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1) && !ariadna)
            {
                updateAriadna(true);
            }
            else if (Input.GetMouseButtonDown(1) && ariadna)
            {
                updateAriadna(false);
            }
        }

        public void updateAriadna(bool ar)
        {
            ariadna = ar;
            segCam.enabled = ariadna;
            contJug.enabled = !ariadna;
        }
    }
}
