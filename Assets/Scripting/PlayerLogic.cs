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
    public float interactDistance = 5f;
    public string inventoryObject = "InventoryObject"; //tag de los objetos que, al recogerlos, van al inventario
    public Image crosshairImage; //referencia a la imagen de la mirilla
    public Color defaultColor = new Color(1f, 1f, 1f, 0.5f); //blanco semitransparente
    public Color interactColor = new Color(1f, 0f, 0f, 0.8f); //rojo más sólido

    [Header("Mirilla dinámica")]
    public float crosshairShrinkSize = 1f; //tamaño encogido de la mirilla cuando apunta a un objeto interactuable
    public float crosshairNormalSize = 1.5f; //tamaño normal de la mirilla
    public float crosshairLerpSpeed = 10f; //velocidad de la animación de la mirilla
    public float pulseSpeed = 2f; //velocidad del parpadeo de la mirilla cuando apunta a objetos interacuables
    public float pulseAmount = 0.1f; //latido del tamaño de la mirilla

    [Header("Inventario")]
    public GameObject inventoryPanel;
    private bool isInventoryOpen = false; //boolean que controla si el inventario está abierto o no
    public GameObject inspectPanel; //variable para el panel de inspección de objetos

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

        //asegurarse de que el inventario esté cerrado al inicio
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isInventoryOpen && !inspectPanel.activeSelf) //solo se puede mover el personaje y rotar la cámara cuando el inventario y el panel de inspección de objetos están cerrados
        {
            Move(); //movimiento del jugador
            Look(); //movimiento de la cámara del jugador
        }
        
        HandleCrosshair(); //actualización de la mirilla

        if (Input.GetMouseButtonDown(0)) //interacción con el objeto al hacer clic
        {
            TryInteract();
        }

        //abre o cierra el inventario con la tecla "Q" siempre que el inspectPanel esté cerrado también
        if (Input.GetKeyDown(KeyCode.Q) && !inspectPanel.activeSelf)
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(1))
        {
            InventoryManager.Instance.UnequipItem();
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

    //método para manejar las acciones de la mirilla
    private void HandleCrosshair()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        bool isLookingAtInteractable = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(inventoryObject))
            {
                crosshairImage.color = interactColor;
                isLookingAtInteractable = true;
            }
            else
            {
                crosshairImage.color = defaultColor;
            }
        }
        else
        {
            crosshairImage.color = defaultColor;
        }

        //animar tamaño de la mirilla
        float baseTargetSize = isLookingAtInteractable ? crosshairShrinkSize : crosshairNormalSize;

        //si está apuntando a un objeto, le añadimos un efecto de latido (pulse)
        if (isLookingAtInteractable)
        {
            baseTargetSize += Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        }

        crosshairImage.rectTransform.localScale = Vector3.Lerp(
            crosshairImage.rectTransform.localScale,
            Vector3.one * baseTargetSize,
            Time.deltaTime * crosshairLerpSpeed
        );
    }

    //método para intentar interactuar con el objeto al hacer clic
    private void TryInteract()
    {
        //creación de un rayo (Ray) que empieza desde la posición del jugador y va hacia delante en la posición hacia la que mira
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        //declara una variable, hit, que guarda la información del objeto si golpea alguno
        RaycastHit hit;

        //lanza el rayo en la escena usando RayCast.Si golpea un collider dentro de la distancia definida en interactDistance, entra en el if.
        //el out hit guarda los datos del impaxcto (qué objeto se golpeó, su posición, etc...)
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(inventoryObject)) //verifica si el objeto impactado tiene el tag InteractableObject
            {
                Debug.Log("Recogido: " + hit.collider.name);

                //obtiene del objeto golpeado su componente CollectableObject con su información
                CollectableObject obj = hit.collider.GetComponent<CollectableObject>();
                if (obj != null)
                {
                    //llamamos al singleton de InventoryManager para agregar ese itemData al inventario
                    InventoryManager.Instance.AddItem(obj.itemData);
                }
                Destroy(hit.collider.gameObject); //destruye el objeto interactuable, dado que ahora está en el inventario
            }
            else
            {
                Debug.Log("No es interactuable");
            }
        }
    }

    //método para abrir/cerrar el inventario
    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            isInventoryOpen = !isInventoryOpen; //cambia el estado del inventario

            inventoryPanel.SetActive(isInventoryOpen);

            //si el inventario está abierto, desbloquea el cursor
            if (isInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                //desactivar la mirilla del juego cuando se abra el inventario
                if (crosshairImage != null)
                {
                    crosshairImage.gameObject.SetActive(false);
                }
            }
            else
            {
                //si el inventario está cerrado, bloquea el cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                //activar la mirilla del juego cuando se cierre el inventario
                if (crosshairImage != null)
                {
                    crosshairImage.gameObject.SetActive(true);
                }
            }
        }
    }
}
