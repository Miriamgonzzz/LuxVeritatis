using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject player;
    private bool isPaused = false;
    private PlayerLogic playerController;
    public DiaryManager diaryManager; //referencia al diaryManager para cerrar el diario si abrimos el menú de pausa

    void Start()
    {
        pauseMenuUI.SetActive(false);
        if (player != null)
            playerController = player.GetComponent<PlayerLogic>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        if (playerController != null)
            playerController.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        //si el diario está abierto, al abrir el menú de pausa se cierra
        if (diaryManager != null && diaryManager.IsDiaryOpen())
        {
            diaryManager.CloseDiary();
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        if (playerController != null)
            playerController.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
