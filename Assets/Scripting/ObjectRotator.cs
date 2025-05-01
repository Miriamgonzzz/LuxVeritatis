using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    //velocidad de rotaci�n
    public float rotationSpeed = 2000f;

    void Update()
    {
        //permite rotar el objeto mientras mantenemos pulsado el bot�n izquierdo del rat�n
        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
    }
}
