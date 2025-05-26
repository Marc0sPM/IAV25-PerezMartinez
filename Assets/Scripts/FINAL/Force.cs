using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Force : MonoBehaviour
{
    [Header("Force Settings")]
    public float force = 2f;
    static public float maxForce = 10f;
    public TextMeshProUGUI forceText;
    private void Update()
    {
        if(forceText)
            forceText.text = force.ToString();
    }
    // Normaliza la fuerza entre 0 y 1
    public float normalizedForce
    {
        get
        {
            if (maxForce <= 0f) return 0f;
            return Mathf.Clamp01(force / maxForce);
        }
    }
}