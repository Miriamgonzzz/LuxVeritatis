using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NoteJournal : MonoBehaviour
{
    [Header("UI")]
    public GameObject journalUI; // Activa/desactiva todo el diario
    public GameObject crosshair;
    public Transform noteListContainer; // Donde van los botones de las notas
    public TextMeshProUGUI noteText; // Donde se muestra el contenido
    public GameObject noteButtonPrefab; // Prefab de botón de nota

    [Header("Gameplay")]
    public MonoBehaviour cameraScript; // Script de control de cámara a desactivar

    private List<string> notes = new List<string>();
    private bool isOpen = false;

    void Start()
    {
        journalUI.SetActive(false);

        // Simulamos recoger 3 notas
        AddNote("Nota 1: El bosque está lleno de susurros.");
        AddNote("Nota 2: Vi una figura oscura cerca del pozo.");
        AddNote("Por medio de la presente, me permito dirigirme a ustedes con el fin de comunicar una situación que considero importante expresar de manera formal y respetuosa. En los últimos días he atravesado una serie de circunstancias personales que han impactado directamente en mis responsabilidades y en mi desempeño habitual. Por respeto al compromiso adquirido y con la intención de mantener una comunicación clara y transparente, considero necesario explicar algunos aspectos relevantes.Durante la última semana, he enfrentado algunos retos de índole personal y familiar que me han exigido tiempo, atención y energía emocional. Esta situación, aunque temporal, ha dificultado mi capacidad para cumplir con algunas de las tareas y compromisos previamente establecidos. Quiero enfatizar que no ha sido mi intención generar inconvenientes, omisiones o malentendidos, y asumo con total responsabilidad las consecuencias que esto haya podido ocasionar. Estoy tomando las medidas necesarias para reorganizar mis tiempos y obligaciones, con el fin de recuperar el ritmo de trabajo habitual y responder de manera eficaz a los requerimientos pendientes. Asimismo, agradezco profundamente la comprensión, el apoyo y la paciencia que puedan brindarme durante este proceso de ajuste. Mi compromiso con este equipo, institución o comunidad se mantiene firme, y estoy plenamente dispuesto/a a redoblar esfuerzos para enmendar cualquier posible afectación.Agradezco su atención a esta nota y quedo a disposición para dialogar más a fondo si se considera oportuno. Reitero mi voluntad de continuar aportando de manera positiva y responsable.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            journalUI.SetActive(isOpen);
            crosshair.SetActive(!isOpen);
            ToggleCursor(isOpen);

            if (cameraScript != null)
                cameraScript.enabled = !isOpen;
        }
    }

    private void ToggleCursor(bool enable)
    {
        if (enable)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddNote(string newNote)
    {
        notes.Add(newNote);

        GameObject newButton = Instantiate(noteButtonPrefab, noteListContainer);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = "Nota " + notes.Count;
        int noteIndex = notes.Count - 1;

        newButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(SetNoteTextSmooth(notes[noteIndex]));
        });
    }

    private IEnumerator SetNoteTextSmooth(string content)
    {
        yield return null; // Espera un frame para evitar que el ScrollRect se reinicie
        noteText.text = content;
    }
}
