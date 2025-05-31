using UnityEngine;

public class LockPuzzle : MonoBehaviour
{
    [Header("Cerraduras físicas en la puerta")]
    public GameObject[] lockVariants; //las 5 cerraduras colocadas en la escena
    private int selectedLockIndex = -1;

    [Header("Puerta")]
    public GameObject door; //la puerta que se desactiva al abrirse (hay que cambiarla por una animación)
    public Animator animator;

    private int puzzlePoints = 100;

    private void Start()
    {
        //desactiva todas las cerraduras al inicio
        foreach (var lockObj in lockVariants)
        {
            if (lockObj != null)
            {
                lockObj.SetActive(false);
            }
                
        }

        //activa una cerradura aleatoria de entre las 5
        selectedLockIndex = Random.Range(0, lockVariants.Length);
        if (lockVariants[selectedLockIndex] != null)
        {
            lockVariants[selectedLockIndex].SetActive(true);
            Debug.Log("Activada la cerradura: " + selectedLockIndex);
        }
        else
        {
            Debug.LogError("La cerradura seleccionada está vacía en el array.");
        }
    }

    //interactuar con la puerta y la llave
    public void TryInteract()
    {
        if (!InventoryManager.Instance.HasItemEquipped())
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("La puerta está cerrada. Busca la llave correcta");
            return;
        }

        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        KeyMetadata keyMeta = equippedObject.GetComponent<KeyMetadata>();

        if (keyMeta != null)
        {
            if (keyMeta.lockIndex == selectedLockIndex)
            {
                FindFirstObjectByType<PlayerLogic>().ShowAdvice("¡Cerradura desbloqueada!"); //para enviar un mensaje al HUD del jugador
                FindFirstObjectByType<PlayerLogic>().AddPoints(puzzlePoints); //para actualizar la puntuación del jugador, pasándole los puntos que ha ganado
                UnlockDoor();
                InventoryManager.Instance.UnequipItem();
            }
            else
            {
                FindFirstObjectByType<PlayerLogic>().ShowAdvice("La llave no coincide con la cerradura");
                puzzlePoints -= 10;
            }
        }
        else
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("Tienes que equiparte con una llave");
        }
    }

    //método para desbloquear la puerta (aqui hay que poner alguna animación)
    public void UnlockDoor()
    {
        if (door != null)
        {
            lockVariants[selectedLockIndex].SetActive(false);
            animator.SetBool("Abrir",true);
        }
        else
        {
            Debug.LogWarning("No se ha asignado un objeto de puerta");
        }
    }

    public int GetSelectedLockIndex()
    {
        return selectedLockIndex;
    }
}
