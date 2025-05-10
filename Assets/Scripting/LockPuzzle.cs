using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class LockPuzzle : MonoBehaviour
{
    [Header("Cerradura")]
    public GameObject lockModel; // Cerradura base (la de la puerta)
    public GameObject[] availableLocks; // Cerraduras diferentes para mostrar en primer plano

    [Header("Llaves")]
    public CollectibleItem[] allKeys; // Lista de todas las llaves posibles del puzzle

    [Header("UI")]
    public Text puzzleMessage; // Texto para mostrar mensajes al jugador
    public Button lockButton; // Botón que se pulsa para intentar abrir
    public GameObject door; // La puerta que se abrirá si acierta

    private int selectedLockIndex = -1; // Cerradura seleccionada al iniciar el puzzle

    private void Start()
    {
        // Seleccionar una cerradura aleatoria
        selectedLockIndex = Random.Range(0, availableLocks.Length);

        // Activar la cerradura seleccionada y ocultar las demás
        UpdateLockUI();

        // Añadir función al botón
        lockButton.onClick.AddListener(TryUnlock);
    }

    private void UpdateLockUI()
    {
        for (int i = 0; i < availableLocks.Length; i++)
        {
            availableLocks[i].SetActive(i == selectedLockIndex);
        }

        if (lockModel != null)
        {
            lockModel.SetActive(false); // Ocultar modelo base si se usa una cerradura ampliada
        }
    }

    private void TryUnlock()
    {
        if (!InventoryManager.Instance.HasItemEquipped())
        {
            ShowMessage("No tienes una llave equipada.");
            return;
        }

        // Obtener GameObject equipado
        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        KeyMetadata keyMeta = equippedObject != null ? equippedObject.GetComponent<KeyMetadata>() : null;

        if (keyMeta != null && keyMeta.lockIndex == selectedLockIndex)
        {
            UnlockDoor();
        }
        else
        {
            ShowMessage("La llave no es correcta.");
        }
    }


    private void UnlockDoor()
    {
        ShowMessage("¡La cerradura se ha abierto!");
        if (door != null)
        {
            door.SetActive(false); // Ocultar la puerta (se abre)
        }

        // Eliminar las llaves del inventario (sean recogidas o no)
        InventoryManager.Instance.RemoveAllItemsOfType("Key");

        // Desequipar la llave
        InventoryManager.Instance.UnequipItem();

        // Aquí puedes llamar a otra función para cambiar de zona si quieres
    }

    private void ShowMessage(string message)
    {
        if (puzzleMessage != null)
        {
            puzzleMessage.text = message;
        }
    }
}
