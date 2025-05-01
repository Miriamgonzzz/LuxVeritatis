using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //singleton, permite acceder a InventoryManager.Instance desde cualquier parte del c�digo y asegura una �nica instancia global
    public static InventoryManager Instance;

    public Transform inventoryUIGrid; //contenedor de los slots del inventario
    public GameObject inventorySlotPrefab; //slot individual del inventario
    public GameObject inspectPanel; //panel de UI que se muestra al inspeccionar un objeto
    public RawImage inspectRenderImage; //RawImage donde se mostrar� el objeto en 3D inspeccionado y renderizado por una c�mara aparte

    public Camera inspectCamera; //c�mara que renderiza solamente el objeto a inspeccionar
    public Transform inspectSpawnPoint; //punto d�nde se instancia temporalmente ese objeto a inspeccionar

    private List<InventoryItem> items = new List<InventoryItem>(); //lista de objetos recogidos por el jugador
    private GameObject currentInspectObject; //referencia al objeto que est� siendo inspeccionado actualmente
    public GameObject inventoryPanel; //campo para el panel de inventario


    //al cargar el script, se asigna this como la instancia global para usar el singleton
    void Awake()
    {
        Instance = this;
    }

    //m�todo para a�adir objetos al inventario
    public void AddItem(InventoryItem newItem)
    {
        if (newItem == null) 
        {
            Debug.LogWarning("Item nulo al intentar a�adir al inventario.");
            return;
        } 

        items.Add(newItem);
        GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIGrid); //creaci�n de un nuevo slot del inventario como hijo de inventoryUIGrid
        slot.GetComponentInChildren<Image>().sprite = newItem.icon; //establece el icono del item (definido en el ScriptableObject) en la imagen del slot
        slot.GetComponent<Button>().onClick.AddListener(() => ShowInspect(newItem)); //a�ade un listener al bot�n del slot para llamar al m�todo ShowInspect con el objeto asociado
    }

    //m�todo para mostrar el objeto en el panel de inspecci�n
    public void ShowInspect(InventoryItem item)
    {
        //elimina cualquier objeto previamente inspeccionado
        if (currentInspectObject != null) 
        {
            Destroy(currentInspectObject);
        }

        Debug.Log("Instanciando: " + item.prefabToInspect.name);
        currentInspectObject = Instantiate(item.prefabToInspect, inspectSpawnPoint); //instancia el objeto a inspeccionar en la posici�n del inspectSpawnPoint

        //asegura que el objeto est� bien posicionado y centrado dentro del spawn
        currentInspectObject.transform.localPosition = Vector3.zero;
        currentInspectObject.transform.localRotation = Quaternion.identity;

        //si el inventario est� abierto, lo cierra al abrir el panel de inspecci�n
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        //escalar el objeto para que sea visible en la vista de inspecci�n
        ScaleObjectToFit(currentInspectObject);

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

    //m�todo para reescalar los objetos en el modo inspecci�n (dado que se ven m�s peque�os en escena)
    private void ScaleObjectToFit(GameObject obj)
    {
        //obtener el renderer del objeto
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            //obtener los l�mites del objeto
            Bounds objectBounds = renderer.bounds;

            //calcular un factor de escala basado en el tama�o del objeto
            float maxSize = Mathf.Max(objectBounds.size.x, objectBounds.size.y, objectBounds.size.z);

            //escala de referencia para el objeto
            float targetScale = 200f; 

            //calcular el factor de escala
            float scaleFactor = targetScale / maxSize;

            //aplicar la escala
            obj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
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
