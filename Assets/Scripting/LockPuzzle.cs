using UnityEngine;

public class LockPuzzle : MonoBehaviour
{
    [Header("Cerraduras físicas en la puerta")]
    public GameObject[] lockVariants; //las 5 cerraduras colocadas en la escena
    private int selectedLockIndex = -1;

    [Header("Puerta")]
    public GameObject door; //la puerta que se abre al solucionar el puzzle
    public Animator animator;

    [Header("SFX")]
    public AudioSource narrationSource; //audioSource para las frases de Elisa
    public AudioClip wrongObjectPhrase; //clip de objeto incorrecto (Script de puzzle cerradura)
    public AudioSource audioSource; //source para los audios de sonidos
    public AudioClip openDoorSound; //sonido de puerta abriéndose
    public AudioClip closeDoorSound; //sonido de puerta bloqueada

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
            AudioSource.PlayClipAtPoint(closeDoorSound, transform.position);
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
                AudioSource.PlayClipAtPoint(openDoorSound, transform.position);
                UnlockDoor();
                InventoryManager.Instance.UnequipItem();
            }
            else
            {
                FindFirstObjectByType<PlayerLogic>().ShowAdvice("La llave no coincide con la cerradura");
                AudioSource.PlayClipAtPoint(closeDoorSound, transform.position);
                narrationSource.clip = wrongObjectPhrase;
                narrationSource.Play();
                puzzlePoints -= 10;
            }
        }
        else
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("Tienes que equiparte con una llave");
        }
    }

    //método para abrir la puerta
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
