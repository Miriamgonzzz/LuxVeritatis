using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTutorial : MonoBehaviour
{
    public GameObject introCanvas;
    public MonoBehaviour cameraController;
    public MonoBehaviour playerMovement;

    void Start()
    {
        // Activar el canvas
        introCanvas.SetActive(true);

        // Mostrar y desbloquear el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraController.enabled = false;
        playerMovement.enabled = false;


        // Pausar el juego (opcional)
        Time.timeScale = 0f;
    }

    public void CerrarCanvas()
    {
        // Ocultar el canvas
        introCanvas.SetActive(false);

        // Ocultar y bloquear el cursor nuevamente
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraController.enabled = true;
        playerMovement.enabled = true;

        // Reanudar el juego
        Time.timeScale = 1f;
    }
}
