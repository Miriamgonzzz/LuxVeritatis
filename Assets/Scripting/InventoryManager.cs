using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //singleton, permite acceder a InventoryManager.Instance desde cualquier parte del código y asegura una única instancia global
    public static InventoryManager Instance;

    [Header("Inventario")]
    public Transform inventoryUIGrid; //contenedor de los slots del inventario
    public GameObject inventorySlotPrefab; //slot individual del inventario
    public GameObject inspectPanel; //panel de UI que se muestra al inspeccionar un objeto
    public RawImage inspectRenderImage; //RawImage donde se mostrará el objeto en 3D inspeccionado y renderizado por una cámara aparte

    public Camera inspectCamera; //cámara que renderiza solamente el objeto a inspeccionar
    public Transform inspectSpawnPoint; //punto dónde se instancia temporalmente ese objeto a inspeccionar

    private List<CollectibleItem> inventoryItems = new List<CollectibleItem>(); //lista de objetos del inventario
    private List<CollectibleItem> secundaryItems = new List<CollectibleItem>(); //lista de objetos coleccionables
    private List<CollectibleItem> textItems = new List<CollectibleItem>(); //lista de objetos escritos (notas y entradas del diario)
    private GameObject currentInspectObject; //referencia al objeto que está siendo inspeccionado actualmente
    public GameObject inventoryPanel; //campo para el panel de inventario

    [Header("Objeto equipado")]
    public Transform handSlot; //punto como si fuera la mano de Elisa
    private GameObject equippedObject; //referencia al objeto actualmente equipado
    private string equippedItemID; //ID del objeto actualmente equipado


    //al cargar el script, se asigna this como la instancia global para usar el singleton
    void Awake()
    {
        //con esta sencilla comprobación, si accidentalmente hay más de un objeto en escena de este tipo, lo destruye dejando uno
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        //verificar si el panel de inspección está activo
        if (inspectPanel.activeSelf)
        {
            //obtener la entrada del ratón (la rueda para hacer scroll en los objetos que inspeccionamos)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                //aumentar o reducir el tamaño del objeto inspeccionado
                if (currentInspectObject != null)
                {
                    float scaleChange = 1f + scroll; //la cantidad de cambio en la escala 
                    currentInspectObject.transform.localScale *= scaleChange; //cambiar la escala del objeto
                }
            }
        }
    }

    //método para añadir objetos al inventario
    public void AddItem(CollectibleItem newItem)
    {
        if (newItem == null) 
        {
            Debug.LogWarning("Item nulo al intentar añadir al inventario.");
            return;
        }

        //dependiendo del itemType, se añade a la lista correspondiente
        if (newItem.itemType == "inventoryItem")
        {
            Debug.Log("Añadido objeto al inventario");
            inventoryItems.Add(newItem); //añadir a la lista de objetos del inventario

            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIGrid); //creación de un nuevo slot del inventario como hijo de inventoryUIGrid
            slot.GetComponentInChildren<Image>().sprite = newItem.icon; //establece el icono del item (definido en el ScriptableObject) en la imagen del slot
            slot.GetComponent<Button>().onClick.AddListener(() => ShowInspect(newItem)); //añade un listener al botón del slot para llamar al método ShowInspect con el objeto asociado
        }
        else if (newItem.itemType == "secundaryItem")
        {
            Debug.Log("Añadido objeto coleccionable al diario");
            secundaryItems.Add(newItem); //añadir a la lista de objetos coleccionables, en el diario
        }
        else if (newItem.itemType == "textItem")
        {
            Debug.Log("Añadido texto al diario");
            textItems.Add(newItem); //añadir a la lista de textos del diario
        }
        else
        {
            Debug.LogWarning("Item con un tipo desconocido: " + newItem.itemType);
        }
    }

    //método para mostrar el objeto en el panel de inspección
    public void ShowInspect(CollectibleItem item)
    {
        //elimina cualquier objeto previamente inspeccionado
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }

        currentInspectObject = Instantiate(item.prefabToInspect, inspectSpawnPoint); //instancia el objeto a inspeccionar en la posición del inspectSpawnPoint
        currentInspectObject.AddComponent<InventoryItemPreview>().sourcePrefab = item.prefabToInspect;//usamos el script de InventoryItemPreview para guardar una preview que podernos equipar si elegimos el objeto

        //asegura que el objeto esté bien posicionado y centrado dentro del spawn
        currentInspectObject.transform.localPosition = Vector3.zero;
        currentInspectObject.transform.localRotation = Quaternion.identity;

        //si el inventario está abierto, lo cierra al abrir el panel de inspección
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        //ajusta la cámara de inspección del objeto dependiendo del tamaño del objeto a inspeccionar en sí
        AdjustInspectCameraToFit(currentInspectObject);

        //asegurarse de que el objeto puede rotar
        ObjectRotator rotator = currentInspectObject.GetComponent<ObjectRotator>();
        if (rotator == null)
        {
            //si el objeto no tiene el script de rotación, se lo añade
            rotator = currentInspectObject.AddComponent<ObjectRotator>();
        }

        //muestra el panel de inspección
        inspectPanel.SetActive(true);
            
    }

    //método para ajustar la cámara de inspección del objeto dependiendo del tamaño del objeto a inspeccionar
    private void AdjustInspectCameraToFit(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No se ha encontrado un renderer sobre el que ajustar la cámara de inspección");
            return;
        }

        Bounds bounds = renderer.bounds;
        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        //distancia óptima basada en el tamaño del objeto y el campo de visión de la cámara
        float distance = objectSize / Mathf.Tan(Mathf.Deg2Rad * inspectCamera.fieldOfView * 0.5f);

        //ajusta la posición en el eje Z de la cámara para alejarla lo suficiente
        Vector3 direction = inspectCamera.transform.forward;
        inspectCamera.transform.position = inspectSpawnPoint.position - direction * distance;

        inspectCamera.transform.LookAt(bounds.center);
    }

    //método para equipar el objeto seleccionado
    public void EquipCurrentItem()
    {
        if (currentInspectObject == null)
        {
            return;
        }

        //borra el objeto anterior si hay uno equipado
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        //instancia el nuevo objeto en la mano de Elisa
        equippedObject = Instantiate(currentInspectObject.GetComponent<InventoryItemPreview>().sourcePrefab, handSlot);
        equippedObject.transform.localPosition = Vector3.zero;
        equippedObject.transform.localRotation = Quaternion.identity;

        //obtener el ID desde el CollectibleItem correspondiente
        CollectibleItem sourceItem = currentInspectObject.GetComponent<InventoryItemPreview>().sourcePrefab.GetComponent<CollectibleItem>();
        if (sourceItem != null)
        {
            equippedItemID = sourceItem.ID;
            Debug.Log("Objeto equipado: " + equippedItemID);
        }

        CloseInspect();
    }

    //método para desequipar el objeto del inventario y poner en null el ID del objeto equipado
    public void UnequipItem()
    {
        if (equippedObject != null)
        {
            Destroy(equippedObject);
            equippedObject = null;
        }

        equippedItemID = null;
    }

    //getter para obtener el ID del objeto equipado
    public string GetEquippedItemID()
    {
        return equippedItemID;
    }

    //método para saber si tenemos un objeto equipado
    public bool HasItemEquipped()
    {
        return !string.IsNullOrEmpty(equippedItemID);
    }

    //método para eliminar del inventario todos los items de un tipo
    public void RemoveAllItemsOfType(string type)
    {
        inventoryItems.RemoveAll(item => item.itemType == type);

        // Opcional: refrescar UI
        foreach (Transform child in inventoryUIGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (CollectibleItem item in inventoryItems)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIGrid);
            slot.GetComponentInChildren<Image>().sprite = item.icon;
            slot.GetComponent<Button>().onClick.AddListener(() => ShowInspect(item));
        }
    }



    //método para cerrar el panel de inspección
    public void CloseInspect()
    {
        //elimina el objeto inspeccionado (como cuando se abre el panel, doble comprobación por si acaso)
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }
        
        //oculta el panel de inspección
        inspectPanel.SetActive(false);

        //reabre el inventario al cerrar el panel de inspección de objeto
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }
}
