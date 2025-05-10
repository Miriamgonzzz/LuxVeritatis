using UnityEngine;

public class LockPuzzle : MonoBehaviour
{
    [Header("Cerraduras físicas en la puerta")]
    public GameObject[] lockVariants; // Las 5 cerraduras colocadas en la escena
    private int selectedLockIndex = -1;

    [Header("Puerta")]
    public GameObject door; // Objeto puerta que se desactiva al abrir

    private void Start()
    {
        // Desactiva todas las cerraduras al inicio
        foreach (var lockObj in lockVariants)
        {
            if (lockObj != null)
                lockObj.SetActive(false);
        }

        // Activa una cerradura aleatoria
        selectedLockIndex = Random.Range(0, lockVariants.Length);
        if (lockVariants[selectedLockIndex] != null)
        {
            lockVariants[selectedLockIndex].SetActive(true);
        }
        else
        {
            Debug.LogError("La cerradura seleccionada está vacía en el array.");
        }
    }

    public void TryInteract()
    {
        if (!InventoryManager.Instance.HasItemEquipped())
        {
            Debug.Log("No tienes una llave equipada.");
            return;
        }

        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        KeyMetadata keyMeta = equippedObject.GetComponent<KeyMetadata>();

        if (keyMeta != null)
        {
            if (keyMeta.lockIndex == selectedLockIndex)
            {
                Debug.Log("¡Cerradura desbloqueada!");
                UnlockDoor();
                InventoryManager.Instance.UnequipItem();
            }
            else
            {
                Debug.Log("La llave no coincide con esta cerradura.");
            }
        }
        else
        {
            Debug.Log("El objeto equipado no es una llave válida.");
        }
    }

    public void UnlockDoor()
    {
        if (door != null)
        {
            door.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se ha asignado un objeto de puerta.");
        }
    }

    public int GetSelectedLockIndex()
    {
        return selectedLockIndex;
    }
}
