using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerLogic : MonoBehaviour
{
    //variables de movimiento
    [Header("Movimiento y cámara")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;

    //variables de interacción
    [Header("Interacción")]
    public float interactDistance = 3f;
    public string interactableTag = "InteractableObject"; //tag de los objetos interactuables
    public Image crosshairImage;          //referencia a la imagen de la mirilla
    public Color defaultColor = new Color(1f, 1f, 1f, 0.5f); //blanco semitransparente
    public Color interactColor = new Color(1f, 0f, 0f, 0.8f); //rojo más sólido

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        //bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshairImage != null)
        {
            crosshairImage.color = defaultColor;
        }
    }

    private void Update()
    {
        Move(); //movimiento del jugador
        Look(); //movimiento de la cámara del jugador
        HandleCrosshair(); //actualización de la mirilla

        if (Input.GetMouseButtonDown(0)) //interacción con el objeto al hacer clic
        {
            TryInteract();
        }
    }

    //método del movimiento del jugador
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    //método para la rotación de la cámara en primera persona
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); //rotación de la cámara en el eje Y (horizontal)

        verticalRotation -= mouseY; //rotación de la cámara en el eje X (vertical)
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    //método para manejar la mirilla de apuntado
    private void HandleCrosshair()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        //si hay algo en el raycast
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            //si ese algo tiene el tag de InteractableObject, la mirilla se vuelve roja. Si no, permanece en blanco
            crosshairImage.color = hit.collider.CompareTag(interactableTag) ? interactColor : defaultColor;
            
        }
        else
        {
            crosshairImage.color = defaultColor; //si no hay nada en el raycast, color por defecto de la mirilla, en blanco
        }
    }

    //método para intentar interactuar con el objeto al hacer clic
    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(interactableTag)) //verifica si el objeto tiene el tag InteractableObject
            {
                Debug.Log("Recogido: " + hit.collider.name);
                Destroy(hit.collider.gameObject); //destruye el objeto interactuable
            }
            else
            {
                Debug.Log("No es interactuable");
            }
        }
    }
}
