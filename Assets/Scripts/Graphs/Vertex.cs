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
    using System;

    // Puntos representativos o vértice (común a todos los esquemas de división, o a la mayoría de ellos)
    // En el pseudocódigo de Millington se habla más bien de NodeRecord, en vez de Vertex y además de guardar el id del nodo y el coste f, se guarda el del padre y también el coste g
    [System.Serializable]
    public class Vertex : MonoBehaviour, IComparable<Vertex>
    {
        /// <summary>
        /// Identificador del vértice/nodo 
        /// </summary>
        public int id;

        /// <summary>
        /// Coste (total estimado) del vértice 
        /// </summary>
        public float cost;

        public int CompareTo(Vertex other)
        {
            float result = this.cost - other.cost;
            return (int)(Mathf.Sign(result) * Mathf.Ceil(Mathf.Abs(result)));
        }

        public bool Equals(Vertex other)
        {
            return (other.id == this.id);
        }

        public override bool Equals(object obj)
        {
            Vertex other = (Vertex)obj;
            if (ReferenceEquals(obj, null)) return false;
            return (other.id == this.id);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
