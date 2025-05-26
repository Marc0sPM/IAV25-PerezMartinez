using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return; 

        transform.forward = mainCamera.transform.forward;
    }
}
