using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTutorial : MonoBehaviour
{
    public GameObject introCanvas;
    public MonoBehaviour cameraController;
    public MonoBehaviour playerMovement;

    [Header("SFX")]
    public AudioSource narrationSource; //audioSource para las frases de Elisa
    public AudioClip lookNotePhrase; //clip para fijarse en la nota del fondo

    void Start()
    {
        //activamos el canvas de controles 
        introCanvas.SetActive(true);

        //mostramos y desbloqueamos el cursor, así como paralizamos el movimiento del jugador
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraController.enabled = false;
        playerMovement.enabled = false;


        //pausamos el juego
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (introCanvas.activeSelf)  //si el canvas del tutorial está activo
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                CerrarCanvas();  //cerramos el tutorial al pulsar Escape, Q o E. Asi evitamos que se abran los demás menús del juego
            }
        }
    }

    public void CerrarCanvas()
    {
        //ocultamos el canvas
        introCanvas.SetActive(false);

        //ocultamos y bloqueamos el cursor, y permitimos el movimiento del personaje
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraController.enabled = true;
        playerMovement.enabled = true;

        //reanudamos el juego
        Time.timeScale = 1f;

        //reproducir primera frase de Elisa al cerrar el Canva
        narrationSource.clip = lookNotePhrase;
        narrationSource.Play();
    }
}
