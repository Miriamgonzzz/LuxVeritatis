using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject[] panels; // Imagen1, Imagen2, Imagen3
    public float[] tiemposDeAparicion; // segundos en los que aparece cada una
    public string nextSceneName = "GameScene"; // Cambia por tu escena real

    private float timer = 0f;
    private int nextPanelIndex = 0;

    void Start()
    {
        // Ocultamos todas las imágenes al inicio
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Mostrar panel cuando toque
        if (nextPanelIndex < panels.Length && timer >= tiemposDeAparicion[nextPanelIndex])
        {
            panels[nextPanelIndex].SetActive(true);
            nextPanelIndex++;
        }

        // Cargar siguiente escena al final de la canción
        if (timer >= 60f) // o pon AudioSource.clip.length si lo haces dinámico
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
