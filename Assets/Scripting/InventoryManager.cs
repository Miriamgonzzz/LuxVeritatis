using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //singleton, permite acceder a InventoryManager.Instance desde cualquier parte del c�digo y asegura una �nica instancia global
    public static InventoryManager Instance;

    [Header("Inventario")]
    public Transform inventoryUIGrid; //contenedor de los slots del inventario
    public GameObject inventorySlotPrefab; //slot individual del inventario
    public GameObject inspectPanel; //panel de UI que se muestra al inspeccionar un objeto
    public RawImage inspectRenderImage; //RawImage donde se mostrar� el objeto en 3D inspeccionado y renderizado por una c�mara aparte

    public Camera inspectCamera; //c�mara que renderiza solamente el objeto a inspeccionar
    public Transform inspectSpawnPoint; //punto d�nde se instancia temporalmente ese objeto a inspeccionar

    private List<CollectibleItem> inventoryItems = new List<CollectibleItem>(); //lista de objetos del inventario
    private List<CollectibleItem> secundaryItems = new List<CollectibleItem>(); //lista de objetos coleccionables
    private List<CollectibleItem> textItems = new List<CollectibleItem>(); //lista de objetos escritos (notas y entradas del diario)
    private GameObject currentInspectObject; //referencia al objeto que est� siendo inspeccionado actualmente
    public GameObject inventoryPanel; //campo para el panel de inventario

    [Header("Objeto equipado")]
    public Transform handSlot; //punto como si fuera la mano de Elisa
    private GameObject equippedObject; //referencia al objeto actualmente equipado
    private string equippedItemID; //ID del objeto actualmente equipado


    //al cargar el script, se asigna this como la instancia global para usar el singleton
    void Awake()
    {
        //con esta sencilla comprobaci�n, si accidentalmente hay m�s de un objeto en escena de este tipo, lo destruye dejando uno
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
        //verificar si el panel de inspecci�n est� activo
        if (inspectPanel.activeSelf)
        {
            //obtener la entrada del rat�n (la rueda para hacer scroll en los objetos que inspeccionamos)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                //aumentar o reducir el tama�o del objeto inspeccionado
                if (currentInspectObject != null)
                {
                    float scaleChange = 1f + scroll; //la cantidad de cambio en la escala 
                    currentInspectObject.transform.localScale *= scaleChange; //cambiar la escala del objeto
                }
            }
        }
    }

    //m�todo para a�adir objetos al inventario
    public void AddItem(CollectibleItem newItem)
    {
        if (newItem == null) 
        {
            Debug.LogWarning("Item nulo al intentar a�adir al inventario.");
            return;
        }

        //dependiendo del itemType, se a�ade a la lista correspondiente
        if (newItem.itemType == "inventoryItem")
        {
            Debug.Log("A�adido objeto al inventario");
            inventoryItems.Add(newItem); //a�adir a la lista de objetos del inventario

            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIGrid); //creaci�n de un nuevo slot del inventario como hijo de inventoryUIGrid
            slot.GetComponentInChildren<Image>().sprite = newItem.icon; //establece el icono del item (definido en el ScriptableObject) en la imagen del slot
            slot.GetComponent<Button>().onClick.AddListener(() => ShowInspect(newItem)); //a�ade un listener al bot�n del slot para llamar al m�todo ShowInspect con el objeto asociado
        }
        else if (newItem.itemType == "secundaryItem")
        {
            Debug.Log("A�adido objeto coleccionable al diario");
            secundaryItems.Add(newItem); //a�adir a la lista de objetos coleccionables, en el diario
        }
        else if (newItem.itemType == "textItem")
        {
            Debug.Log("A�adido texto al diario");
            textItems.Add(newItem); //a�adir a la lista de textos del diario
        }
        else
        {
            Debug.LogWarning("Item con un tipo desconocido: " + newItem.itemType);
        }
    }

    //m�todo para mostrar el objeto en el panel de inspecci�n
    public void ShowInspect(CollectibleItem item)
    {
        //elimina cualquier objeto previamente inspeccionado
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }

        currentInspectObject = Instantiate(item.prefabToInspect, inspectSpawnPoint); //instancia el objeto a inspeccionar en la posici�n del inspectSpawnPoint
        InventoryItemPreview preview = currentInspectObject.AddComponent<InventoryItemPreview>();
        preview.sourcePrefab = item.prefabToInspect;//usamos el script de InventoryItemPreview para guardar una preview que podremos equipar si elegimos el objeto
        preview.sourceItem = item; //aqu� pasamos el ScriptableObject real para poderlo equipar, usar, etc...

        //asegura que el objeto est� bien posicionado y centrado dentro del spawn
        currentInspectObject.transform.localPosition = Vector3.zero;
        currentInspectObject.transform.localRotation = Quaternion.identity;

        //si el inventario est� abierto, lo cierra al abrir el panel de inspecci�n
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        //ajusta la c�mara de inspecci�n del objeto dependiendo del tama�o del objeto a inspeccionar en s�
        AdjustInspectCameraToFit(currentInspectObject);

        //asegurarse de que el objeto puede rotar
        ObjectRotator rotator = currentInspectObject.GetComponent<ObjectRotator>();
        if (rotator == null)
        {
            //si el objeto no tiene el script de rotaci�n, se lo a�ade
            rotator = currentInspectObject.AddComponent<ObjectRotator>();
        }

        //muestra el panel de inspecci�n
        inspectPanel.SetActive(true);
            
    }

    //m�todo para ajustar la c�mara de inspecci�n del objeto dependiendo del tama�o del objeto a inspeccionar
    private void AdjustInspectCameraToFit(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No se ha encontrado un renderer sobre el que ajustar la c�mara de inspecci�n");
            return;
        }

        Bounds bounds = renderer.bounds;
        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        //distancia �ptima basada en el tama�o del objeto y el campo de visi�n de la c�mara
        float distance = objectSize / Mathf.Tan(Mathf.Deg2Rad * inspectCamera.fieldOfView * 0.5f);

        //ajusta la posici�n en el eje Z de la c�mara para alejarla lo suficiente
        Vector3 direction = inspectCamera.transform.forward;
        inspectCamera.transform.position = inspectSpawnPoint.position - direction * distance;

        inspectCamera.transform.LookAt(bounds.center);
    }

    //m�todo para equipar el objeto seleccionado
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
        InventoryItemPreview preview = currentInspectObject.GetComponent<InventoryItemPreview>();
        if (preview != null && preview.sourceItem != null)
        {
            equippedItemID = preview.sourceItem.ID;
            Debug.Log("Objeto equipado: " + equippedItemID);
        }

        CloseInspect();
        FindFirstObjectByType<PlayerLogic>().ToggleInventory();
    }

    //m�todo para desequipar el objeto del inventario y poner en null el ID del objeto equipado
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

    //getter para obtener el objeto equipado (para interactuar con los c�digos de los Puzzles)
    public GameObject GetEquippedObject()
    {
        return equippedObject;
    }


    //m�todo para saber si tenemos un objeto equipado
    public bool HasItemEquipped()
    {
        return !string.IsNullOrEmpty(equippedItemID);
    }

    //m�todo para eliminar objetos del inventario (si se usan en un puzzle, por ejemplo)
    public void RemoveItem(CollectibleItem itemToRemove)
    {
        if (itemToRemove == null || string.IsNullOrEmpty(itemToRemove.ID))
        {
            Debug.LogWarning("Intentando eliminar un objeto nulo o sin ID del inventario.");
            return;
        }

        bool removed = false;

        // 1. Eliminar de listas seg�n su ID
        removed |= RemoveFromListByID(inventoryItems, itemToRemove.ID);
        removed |= RemoveFromListByID(secundaryItems, itemToRemove.ID);
        removed |= RemoveFromListByID(textItems, itemToRemove.ID);

        if (!removed)
        {
            Debug.LogWarning($"El objeto con ID '{itemToRemove.ID}' no se encontr� en ninguna lista del inventario.");
            return;
        }

        // 2. Eliminar el slot de UI correspondiente
        foreach (Transform slot in inventoryUIGrid)
        {
            InventoryItemPreview preview = slot.GetComponent<InventoryItemPreview>();
            if (preview != null && preview.sourceItem != null && preview.sourceItem.ID == itemToRemove.ID)
            {
                Destroy(slot.gameObject);
                break;
            }
        }

        // 3. Si est� equipado, desequiparlo
        if (equippedObject != null)
        {
            InventoryItemPreview equippedPreview = equippedObject.GetComponent<InventoryItemPreview>();
            if (equippedPreview != null && equippedPreview.sourceItem != null && equippedPreview.sourceItem.ID == itemToRemove.ID)
            {
                UnequipItem();
            }
        }
    }


    private bool RemoveFromListByID(List<CollectibleItem> list, string id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].ID == id)
            {
                list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }




    //m�todo para cerrar el panel de inspecci�n
    public void CloseInspect()
    {
        //elimina el objeto inspeccionado (como cuando se abre el panel, doble comprobaci�n por si acaso)
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }
        
        //oculta el panel de inspecci�n
        inspectPanel.SetActive(false);

        //reabre el inventario al cerrar el panel de inspecci�n de objeto
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }
}
