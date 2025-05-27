using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor : MonoBehaviour
{
    public abstract void Actualizar();
    public abstract List<GameObject> ObtenerDatos();
}
