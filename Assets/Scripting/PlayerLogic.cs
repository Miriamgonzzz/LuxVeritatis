using System.Collections;
using TMPro;
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
    public string puzzleToSolve = "PuzzleToSolve"; //tag de los objetos que inician un puzzle al interactuar con ellos
    public string diaryPage = "DiaryPage"; //tag para detectar las páginas del diario
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

    [Header("Hud Jugador")]
    public TextMeshProUGUI adviceText;
    public TextMeshProUGUI playerPoints;

    [Header("Objeto especial: Linterna")]
    public GameObject flashlightPrefab; //prefab de la linterna
    public Transform equipSlot; //dónde se instancia la linterna (en la "mano" del jugador)
    private GameObject equippedFlashlight; //referencia a la linterna equipada
    private bool isFlashlightEquipped = false; //boolean para controlar si está o no equipada

    [Header("Páginas del Diario")]
    public GameObject diaryPage2; //referencia a la segunda página del diario para hacerla aparecer cuando recojamos la primera

    private Quaternion originalHandSlotRotation; //rotación original del HandSlot (para que solo rote 90 grados cuando se equipe la linterna, el resto de objetos que no se roten)
    private int currentPoints = 0;
    private Coroutine currentAdvideCoroutine;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        playerPoints.text = "Puntos: ";

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

        //método para equipar la linterna
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleFlashlight();
        }

        //desequipa el objeto equipado (método dentro del InventoryManager)
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
            if (hit.collider.CompareTag(inventoryObject) || hit.collider.CompareTag(puzzleToSolve) || hit.collider.CompareTag(diaryPage))
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
        //el out hit guarda los datos del impacto (qué objeto se golpeó, su posición, etc...)
        if (Physics.Raycast(ray, out hit, interactDistance))
        {

            if (hit.collider.CompareTag(inventoryObject) && !isInventoryOpen) //verifica si el objeto impactado tiene el tag InventoryObject y el inventario está cerrado
            {

                //obtiene el CollectableObject con su información del objeto golpeado por el ray
                CollectableObject obj = hit.collider.GetComponent<CollectableObject>();

                ShowAdvice("OBJETO RECOGIDO: " + obj.itemData.itemName);

                Debug.Log("Info del objeto: " + obj.itemData.ID + "\n"
                    + obj.itemData.itemName + "\n"
                    + obj.itemData.description + "\n"
                    + obj.itemData.storyText);
                if (obj != null)
                {
                    //llamamos al singleton de InventoryManager para agregar ese itemData al inventario
                    InventoryManager.Instance.AddItem(obj.itemData);
                }
                Destroy(hit.collider.gameObject); //destruye el objeto interactuable, dado que ahora está en el inventario
            }
            else if (hit.collider.GetComponentInParent<LockPuzzle>() && !isInventoryOpen) //busca en el padre del objeto golpeado (puerta o cerraduras) el script del primer puzzle, LockPuzzle, si el inventario está cerrado
            {
                hit.collider.GetComponentInParent<LockPuzzle>().TryInteract();
            }
            else if (hit.collider.CompareTag(diaryPage) && !isInventoryOpen)
            {
                //obtiene el CollectableObject con su información del objeto golpeado por el ray
                CollectableObject obj = hit.collider.GetComponent<CollectableObject>();

                ShowAdvice("OBJETO RECOGIDO: " + obj.itemData.itemName);

                if (obj != null)
                {
                    //llamamos al singleton de InventoryManager para agregar ese itemData al inventario
                    InventoryManager.Instance.AddItem(obj.itemData);
                }

                //activa la segunda página del diario (oculta hasta recoger la primera)
                if (diaryPage2 != null)
                {
                    diaryPage2.SetActive(true);
                }

                Destroy(hit.collider.gameObject); //destruye el objeto interactuable, dado que ahora está en el inventario
            }
            else
            {
                Debug.Log("No es un objeto interactuable");
            }
        }
    }

    //método para abrir/cerrar el inventario
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            //si la linterna está equipada, al abrir el inventario se desequipa para evitar conflictos con otros objetos equipables
            if (isFlashlightEquipped)
            {
                Destroy(equippedFlashlight);
                equipSlot.localRotation = originalHandSlotRotation;
                isFlashlightEquipped = false;
            }

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

    //método para equipar/desequipar la linterna
    private void ToggleFlashlight()
    {
        if (isFlashlightEquipped)
        {
            Destroy(equippedFlashlight);
            isFlashlightEquipped = false;

            equipSlot.localRotation = originalHandSlotRotation;
        }
        else
        {
            InventoryManager.Instance.UnequipItem();

            //instanciar como hija de la cámara
            equippedFlashlight = Instantiate(flashlightPrefab, playerCamera.transform);

            //posicionar a la derecha de la cámara (eje X hacia la derecha, en positivo, eje Y en negativo, hacia abajo, eje Z en positivo, un poco hacia delante)
            equippedFlashlight.transform.localPosition = new Vector3(0.8f, -0.4f, 0.5f);

            //hacer que el eje 'Y' de la linterna apunte hacia delante, en -10f
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * -10f;
            equippedFlashlight.transform.LookAt(targetPosition);

            //corregimos la orientación para que apunte hacia delante
            equippedFlashlight.transform.Rotate(-90f, 0f, 0f);

            isFlashlightEquipped = true;
        }
    }




    public void AddPoints(int amount)
    {
        currentPoints += amount;
        UpdatePointsUI();
    }

    private void UpdatePointsUI()
    {
        if(playerPoints != null)
        {
            playerPoints.text = "Puntos: " + currentPoints;
        }
    }

    public void ShowAdvice(string message, float duration = 5f)
    {
        if(currentAdvideCoroutine != null)
        {
            StopCoroutine(currentAdvideCoroutine);
        }

        currentAdvideCoroutine = StartCoroutine(ShowAdviceCoroutine(message, duration));
    }

    private IEnumerator ShowAdviceCoroutine(string message, float duration)
    {
        adviceText.text = message;
        adviceText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        adviceText.text = "";
        adviceText.gameObject.SetActive(false);
    }
}
