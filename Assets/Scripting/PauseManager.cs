using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject player;
    private bool isPaused = false;
    private PlayerLogic playerController;

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

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;

        // Guardar los items del inventario
        foreach (var item in InventoryManager.Instance.GetInventoryItems())
        {
            data.inventoryItemIDs.Add(item.ID);
        }

        // Guardar el nombre de la escena en la que se encuentra el jugador
        data.sceneName = SceneManager.GetActiveScene().name;

        // Guardar los datos
        SaveSystem.Save(data);
    }

}
