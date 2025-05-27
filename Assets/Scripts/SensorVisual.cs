using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class SensorVisual : Sensor
{
    [Tooltip("Rango de visión del sensor")]
    public float rangoVision;

    [Tooltip("Ángulo de visión del sensor en grados")]
    public float anguloVision = 45f;

    [Tooltip("Cantidad de rayos para mejorar la detección")]
    public int cantidadRayos = 10;

 
    public int playerLayer;
    public int[] obstacleLayers;
    private List<GameObject> objetosDetectados = new List<GameObject>();

    private Material visionMaterial;

    private void Start()
    {
        if (this != null)
        {
            GestorSensorial.Instance.RegistrarSensor(this);
            visionMaterial = new Material(Shader.Find("Unlit/Color"));
            visionMaterial.color = new Color(1f, 07f, 0f, 1f);       
        }
    }
    void OnDisable()
    {
        GestorSensorial.Instance.DesregistrarSensor(this);
    }
    public override void Actualizar()
    {
        objetosDetectados.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangoVision);

        foreach (Collider collider in colliders)
        {
            if (obstacleLayers.Contains(collider.gameObject.layer) || collider.gameObject.layer == playerLayer)
            {
                Vector3 directionToObject = (collider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToObject);

                if (angle <= anguloVision / 2)
                {
                    if (VerificarConRaycasts(collider.transform.position, collider.gameObject.layer))
                    {
                        // Debug.Log($"OBJETO DETECTADO: {collider.gameObject.name} en posición {collider.transform.position} con ángulo {angle}°");
                        objetosDetectados.Add(collider.gameObject);
                    }
                }
            }
        }
    }

    private bool VerificarConRaycasts(Vector3 objetivo, int targetLayer)
    {
        for (int i = 0; i < cantidadRayos; i++)
        {
            float randomAngle = Random.Range(-anguloVision / 2, anguloVision / 2);
            Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, randomDirection);
            RaycastHit hit;

            Debug.DrawRay(transform.position, randomDirection * rangoVision, Color.green, 0.1f);

            if (Physics.Raycast(ray, out hit, rangoVision))
            {
                //Debug.Log($"Raycast impactó en: {hit.collider.gameObject.name}, posición: {hit.point}");

                foreach (int layer in obstacleLayers)
                {
                    if (hit.collider.gameObject.layer == layer)
                    {
                        return false;
                    }
                }

                if (hit.collider.transform.position == objetivo)
                {
                    if (targetLayer == playerLayer)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override List<GameObject> ObtenerDatos()
    {
        return objetosDetectados;
    }

    private void OnRenderObject()
    {
        if (visionMaterial == null)
            return;

        visionMaterial.SetPass(0);
        
        GL.PushMatrix();
        GL.Begin(GL.TRIANGLES);
        GL.Color(visionMaterial.color);

        Vector3 origin = transform.position;
        for (float i = -anguloVision / 2; i < anguloVision / 2; i += 5f)
        {
            Vector3 dirA = Quaternion.Euler(0, i, 0) * transform.forward * rangoVision;
            Vector3 dirB = Quaternion.Euler(0, i + 5f, 0) * transform.forward * rangoVision;

            RaycastHit hitA, hitB;
            bool blockedA = false, blockedB = false;

            if (Physics.Raycast(origin, dirA, out hitA, rangoVision))
            {
                foreach (int layer in obstacleLayers)
                {
                    if (hitA.collider.gameObject.layer == layer)
                    {
                        blockedA = true;
                        break;
                    }
                }
            }

            if (Physics.Raycast(origin, dirB, out hitB, rangoVision))
            {
                foreach (int layer in obstacleLayers)
                {
                    if (hitB.collider.gameObject.layer == layer)
                    {
                        blockedB = true;
                        break;
                    }
                }
            }

            Vector3 endA = blockedA ? hitA.point : (origin + dirA);
            Vector3 endB = blockedB ? hitB.point : (origin + dirB);

            GL.Vertex(origin);
            GL.Vertex(endA);
            GL.Vertex(endB);
        }

        GL.End();
        GL.PopMatrix();
    }
}
