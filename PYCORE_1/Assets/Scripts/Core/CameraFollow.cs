using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadCamara = 5f;

    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicion = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, posicion, velocidadCamara * Time.deltaTime);
        }
    }
}