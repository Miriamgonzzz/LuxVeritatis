using UnityEngine;
using UnityEngine.UI;

public class LockPuzzle : MonoBehaviour
{
    [Header("Cerradura")]
    public GameObject lockModel; // Cerradura base decorativa
    public GameObject[] availableLocks; // Cerraduras ampliadas para mostrar

    [Header("Llaves")]
    public CollectibleItem[] allKeys; // Lista de todas las llaves posibles que pueden abrir esta cerradura

    [Header("UI")]
    public GameObject puzzleCanvas; // El canvas que contiene el puzzle
    public Text puzzleMessage; // Texto para mostrar mensajes
    public Button lockButton; // Botón para intentar abrir
    public GameObject door; // La puerta que se abrirá si la llave es correcta

    private int selectedLockIndex = -1;

    private void Start()
    {
        // Al inicio el canvas del puzzle está oculto
        puzzleCanvas.SetActive(false);

        // Añadir la lógica al botón de intentar abrir la cerradura
        lockButton.onClick.AddListener(TryUnlock);
    }

    // Hacemos este método público para que PlayerLogic pueda acceder a él
    public void OpenPuzzle()
    {
        // Seleccionar una cerradura aleatoria
        selectedLockIndex = Random.Range(0, availableLocks.Length);
        UpdateLockUI();

        // Mostrar el canvas del puzzle
        puzzleCanvas.SetActive(true);
    }

    // Hacemos este método público para que PlayerLogic pueda acceder a él
    public void UpdateLockUI()
    {
        // Activar solo la cerradura seleccionada
        for (int i = 0; i < availableLocks.Length; i++)
        {
            availableLocks[i].SetActive(i == selectedLockIndex);
        }
    }

    public int GetSelectedLockIndex()
    {
        return selectedLockIndex;
    }

    // Hacemos este método público para que PlayerLogic pueda acceder a él
    public void TryUnlock()
    {
        if (!InventoryManager.Instance.HasItemEquipped())
        {
            ShowMessage("No tienes una llave equipada.");
            return;
        }

        // Obtener el objeto equipado
        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        if (equippedObject == null)
        {
            ShowMessage("No se encontró el objeto equipado.");
            return;
        }

        // Verificar si la llave equipada es la correcta
        KeyMetadata keyMeta = equippedObject.GetComponent<KeyMetadata>();
        if (keyMeta != null && keyMeta.lockIndex == selectedLockIndex)
        {
            UnlockDoor(keyMeta);
        }
        else
        {
            ShowMessage("La llave no es correcta.");
        }
    }

    // Hacemos este método público para que PlayerLogic pueda acceder a él
    public void UnlockDoor(KeyMetadata keyMeta)
    {
        ShowMessage("¡La cerradura se ha abierto!");

        // Desactivar la puerta
        if (door != null)
        {
            door.SetActive(false);
        }

        // Buscar la llave correspondiente en la lista de llaves conocidas
        CollectibleItem usedKey = null;
        foreach (CollectibleItem key in allKeys)
        {
            if (key != null && key.prefabToInspect.name == keyMeta.gameObject.name.Replace("(Clone)", "").Trim())
            {
                usedKey = key;
                break;
            }
        }

        if (usedKey != null)
        {
            InventoryManager.Instance.RemoveItem(usedKey);
        }
        else
        {
            Debug.LogWarning("No se encontró la llave usada en la lista de llaves conocidas.");
        }

        InventoryManager.Instance.UnequipItem();
    }

    // Hacemos este método público para que PlayerLogic pueda acceder a él
    public void ShowMessage(string message)
    {
        if (puzzleMessage != null)
        {
            puzzleMessage.text = message;
        }
    }
}
