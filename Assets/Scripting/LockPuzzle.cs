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
    public Text puzzleMessage;
    public Button lockButton;
    public GameObject door;

    private int selectedLockIndex = -1;

    private void Start()
    {
        selectedLockIndex = Random.Range(0, availableLocks.Length);
        UpdateLockUI();
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
            lockModel.SetActive(false);
        }
    }

    private void TryUnlock()
    {
        if (!InventoryManager.Instance.HasItemEquipped())
        {
            ShowMessage("No tienes una llave equipada.");
            return;
        }

        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        if (equippedObject == null)
        {
            ShowMessage("No se encontró el objeto equipado.");
            return;
        }

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

    private void UnlockDoor(KeyMetadata keyMeta)
    {
        ShowMessage("¡La cerradura se ha abierto!");
        if (door != null)
        {
            door.SetActive(false);
        }

        // Buscar la CollectibleItem correspondiente a la llave usada
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

    private void ShowMessage(string message)
    {
        if (puzzleMessage != null)
        {
            puzzleMessage.text = message;
        }
    }
}
