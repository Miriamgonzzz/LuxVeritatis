using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class DiaryManager : MonoBehaviour
{
    public GameObject diaryUI;
    public Button diaryButton;
    public Button missionsButton;
    public Button loreButton;
    public TextMeshProUGUI diaryContentText;
    public GameObject pageButtonPrefab;
    public Transform pageButtonContainer;
    private List<GameObject> activePageButtons = new List<GameObject>();


    public Image mapImage;         //imagen para el mapa
    public Sprite mapSprite;       //sprite del mapa

    private bool isDiaryOpen = false;
    private PlayerLogic playerLogic; //referencia al jugador

    [Header("Inventario")]
    public GameObject inventoryPanel; //referencia al inventario para evitar que el diario se pueda abrir si tenemos el inventario abierto

    [Header("SFX")]
    public AudioSource narrationSource; //audioSource para las frases de Elisa
    public AudioClip diaryPage001; //clip para la primera página del diario de Alvar
    public AudioClip diaryPage002; //clip para la segunda página del diario de Alvar
    public AudioClip diaryPage003; //clip para la tercera página del diario de Alvar
    public AudioClip diaryPage004; //clip para la cuarta página del diario de Alvar

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
            //evita abrir el diario si el inventario está activo
            bool isInventoryOpen = inventoryPanel != null && inventoryPanel.activeSelf;

            //solo permitir abrir el diario si el juego no está pausado Y el inventario NO está abierto,
            //o permitir cerrarlo si ya está abierto
            if ((!isInventoryOpen && Time.timeScale != 0f) || isDiaryOpen)
            {
                ToggleDiary();
            }
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
            ClearPageButtons(true); //limpiamos botones y paramos audio de narración del diario cuando cerramos diario
        }
    }

    void ShowDiary()
    {
        mapImage.gameObject.SetActive(false);
        diaryContentText.gameObject.SetActive(true);

        //Limpia botones anteriores
        ClearPageButtons();

        // Obtener páginas recogidas
        List<CollectibleItem> pages = InventoryManager.Instance.GetTextItems();

        if (pages.Count == 0)
        {
            diaryContentText.text = "No hay páginas recogidas aún...";
        }
        else
        {
            diaryContentText.text = "Haz clic en una página para leerla.\n";
        }

        int pageNumber = 1;

        //crear botones por cada página
        foreach (CollectibleItem page in pages)
        {
            GameObject buttonObj = Instantiate(pageButtonPrefab, pageButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = page.itemName;

            activePageButtons.Add(buttonObj);

            //listener para mostrar la página pulsada
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                //enseñamos el texto
                ShowPageContent(page);
                //ocultamos los botones/páginas, pero no paramos el audio narrado de la página del diario
                ClearPageButtons(false);
            });
        }
    }

    void ShowPageContent(CollectibleItem page)
    {
        diaryContentText.text = $"📖 {page.description}\n\n{page.storyText}";

        //para reproducir el texto de la página del diario seleccionada
        if (page.ID == "diary001")
        {
            Debug.Log("REPRODUCIENDO AUDIO DE NOTA 1");
            narrationSource.clip = diaryPage001;
            narrationSource.Play();
        }
        else if(page.ID == "diary002")
        {
            Debug.Log("REPRODUCIENDO AUDIO DE NOTA 2");
            narrationSource.clip = diaryPage002;
            narrationSource.Play();
        }
        else if(page.ID == "diary003")
        {
            Debug.Log("REPRODUCIENDO AUDIO DE NOTA 3");
            narrationSource.clip = diaryPage003;
            narrationSource.Play();
        }
        else
        {
            Debug.Log("REPRODUCIENDO AUDIO DE NOTA 4");
            narrationSource.clip = diaryPage004;
            narrationSource.Play();
        }

    }


    void ShowMap()
    {
        ClearPageButtons(true); //limpiamos botones y paramos audio de narración del diario
        diaryContentText.gameObject.SetActive(false);
        mapImage.sprite = mapSprite;
        mapImage.gameObject.SetActive(true);
    }

    void ShowLore()
    {
        ClearPageButtons(true); //limpiamos botones y paramos audio de narración del diario
        mapImage.gameObject.SetActive(false);
        diaryContentText.gameObject.SetActive(true);
        diaryContentText.text = "Aún no has recogido ningún coleccionable";
    }

    //método para limpiar los botones de las páginas del diario y detener la narración del diario (le pasamos un booleado que
    //por defecto es true. Se lo pasamos en false cuando no queremos detener la narración de las páginas del diario)
    void ClearPageButtons(bool stopAudio = true)
    {
        if (stopAudio)
        {
            narrationSource.Stop(); //solo para cuando cierres el diario, por ejemplo
        }
        foreach (GameObject btn in activePageButtons)
        {
            Destroy(btn);
        }
        activePageButtons.Clear();
    }

    //método para saber si está el diario abierto desde el código del PauseManager
    public bool IsDiaryOpen()
    {
        return isDiaryOpen;
    }

    public void CloseDiary()
    {
        if (isDiaryOpen)
        {
            ToggleDiary(); //reutiliza el mismo método que ya gestiona el cierre
        }
    }


}
