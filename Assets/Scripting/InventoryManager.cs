using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //singleton, permite acceder a InventoryManager.Instance desde cualquier parte del código y asegura una única instancia global
    public static InventoryManager Instance;

    public Transform inventoryUIGrid; //contenedor de los slots del inventario
    public GameObject inventorySlotPrefab; //slot individual del inventario
    public GameObject inspectPanel; //panel de UI que se muestra al inspeccionar un objeto
    public RawImage inspectRenderImage; //RawImage donde se mostrará el objeto en 3D inspeccionado y renderizado por una cámara aparte

    public Camera inspectCamera; //cámara que renderiza solamente el objeto a inspeccionar
    public Transform inspectSpawnPoint; //punto dónde se instancia temporalmente ese objeto a inspeccionar

    private List<InventoryItem> items = new List<InventoryItem>(); //lista de objetos recogidos por el jugador
    private GameObject currentInspectObject; //referencia al objeto que está siendo inspeccionado actualmente
    public GameObject inventoryPanel; //campo para el panel de inventario


    //al cargar el script, se asigna this como la instancia global para usar el singleton
    void Awake()
    {
        Instance = this;
    }

    //método para añadir objetos al inventario
    public void AddItem(InventoryItem newItem)
    {
        if (newItem == null) 
        {
            Debug.LogWarning("Item nulo al intentar añadir al inventario.");
            return;
        } 

        items.Add(newItem);
        GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIGrid); //creación de un nuevo slot del inventario como hijo de inventoryUIGrid
        slot.GetComponentInChildren<Image>().sprite = newItem.icon; //establece el icono del item (definido en el ScriptableObject) en la imagen del slot
        slot.GetComponent<Button>().onClick.AddListener(() => ShowInspect(newItem)); //añade un listener al botón del slot para llamar al método ShowInspect con el objeto asociado
    }

    //método para mostrar el objeto en el panel de inspección
    public void ShowInspect(InventoryItem item)
    {
        //elimina cualquier objeto previamente inspeccionado
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }

        Debug.Log("Instanciando: " + item.prefabToInspect.name);
        currentInspectObject = Instantiate(item.prefabToInspect, inspectSpawnPoint); //instancia el objeto a inspeccionar en la posición del inspectSpawnPoint

        //asegura que el objeto esté bien posicionado y centrado dentro del spawn
        currentInspectObject.transform.localPosition = Vector3.zero;
        currentInspectObject.transform.localRotation = Quaternion.identity;

        //si el inventario está abierto, lo cierra al abrir el panel de inspección
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        //escalar el objeto para que sea visible en la vista de inspección
        ScaleObjectToFit(currentInspectObject);

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

    //método para reescalar los objetos en el modo inspección (dado que se ven más pequeños en escena)
    private void ScaleObjectToFit(GameObject obj)
    {
        //obtener el renderer del objeto
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            //obtener los límites del objeto
            Bounds objectBounds = renderer.bounds;

            //calcular un factor de escala basado en el tamaño del objeto
            float maxSize = Mathf.Max(objectBounds.size.x, objectBounds.size.y, objectBounds.size.z);

            //escala de referencia para el objeto
            float targetScale = 200f; 

            //calcular el factor de escala
            float scaleFactor = targetScale / maxSize;

            //aplicar la escala
            obj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
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
