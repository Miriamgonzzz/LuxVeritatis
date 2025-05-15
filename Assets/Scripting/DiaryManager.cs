using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryManager : MonoBehaviour
{
    public GameObject diaryUI;
    public Button diaryButton;
    public Button missionsButton;
    public Button loreButton;
    public TextMeshProUGUI diaryContentText;

    public Image mapImage;         // Imagen para el mapa
    public Sprite mapSprite;       // Sprite del mapa

    private bool isDiaryOpen = false;
    private PlayerLogic playerLogic; // Referencia al jugador

    void Start()
    {
        diaryUI.SetActive(false);

        diaryButton.onClick.AddListener(ShowDiary);
        missionsButton.onClick.AddListener(ShowMap);
        loreButton.onClick.AddListener(ShowLore);

        mapImage.gameObject.SetActive(false);

        // Buscamos al jugador en escena
        playerLogic = FindObjectOfType<PlayerLogic>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleDiary();
        }
    }

    void ToggleDiary()
    {
        isDiaryOpen = !isDiaryOpen;
        diaryUI.SetActive(isDiaryOpen);

        if (isDiaryOpen)
        {
            Time.timeScale = 0f; // PAUSA el juego
            playerLogic.enabled = false; // Bloquea al jugador
            Cursor.lockState = CursorLockMode.None; // Libera el cursor
            Cursor.visible = true;
            ShowDiary(); // Muestra contenido por defecto
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el juego
            playerLogic.enabled = true; // Activa al jugador
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ShowDiary()
    {
        mapImage.gameObject.SetActive(false);
        diaryContentText.gameObject.SetActive(true);
        diaryContentText.text = "📖 Entrada del diario:\nHoy he llegado a las ruinas antiguas. Algo extraño se siente en el aire...";
    }

    void ShowMap()
    {
        diaryContentText.gameObject.SetActive(false);
        mapImage.sprite = mapSprite;
        mapImage.gameObject.SetActive(true);
    }

    void ShowLore()
    {
        mapImage.gameObject.SetActive(false);
        diaryContentText.gameObject.SetActive(true);
        diaryContentText.text = "📚 Lore:\nHace siglos, los núcleos de color mantenían el equilibrio mágico del mundo...";
    }
}
