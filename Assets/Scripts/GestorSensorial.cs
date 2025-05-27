using System.Collections.Generic;
using UnityEngine;

public class GestorSensorial : MonoBehaviour
{
    public static GestorSensorial Instance { get; private set; }
    private List<Sensor> sensores = new List<Sensor>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistrarSensor(Sensor sensor)
    {
        sensores.Add(sensor);
    } 
    
    public void DesregistrarSensor(Sensor sensor)
    {
        sensores.Remove(sensor);
    }

    void Update()
    {
        foreach (var sensor in sensores)
        {
            sensor.Actualizar();
        }
    }

    public List<GameObject> ObtenerDatosSensores()
    {
        List<GameObject> datosCombinados = new List<GameObject>();
        foreach (var sensor in sensores)
        {
            datosCombinados.AddRange(sensor.ObtenerDatos());
        }
        return datosCombinados;
    }
}