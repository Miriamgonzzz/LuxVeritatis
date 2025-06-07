using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject[] panels;
    public float[] tiemposDeAparicion;
    public string nextSceneName = "GameScene";

    private float timer = 0f;
    private int nextPanelIndex = 0;

    public AudioSource narrationSource;  //para el audio de la narración
    public AudioSource musicSource;      //para la música de fondo

    public AudioClip audioIntro;
    public AudioClip introMusic;

    void Start()
    {
        //ocultar paneles al inicio
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        //asignar clips a los audio sources y reproducir
        if (narrationSource != null && audioIntro != null)
        {
            narrationSource.clip = audioIntro;
            narrationSource.Play();
        }

        if (musicSource != null && introMusic != null)
        {
            musicSource.clip = introMusic;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (nextPanelIndex < panels.Length && timer >= tiemposDeAparicion[nextPanelIndex])
        {
            panels[nextPanelIndex].SetActive(true);
            nextPanelIndex++;
        }

        //esperar a que termine la narración
        if (narrationSource != null && !narrationSource.isPlaying && timer > 2f) //para evitar bug por no iniciar
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
