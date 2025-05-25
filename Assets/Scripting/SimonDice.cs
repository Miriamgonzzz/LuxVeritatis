using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SimonDice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button[] colorButtons; // 0=Red, 1=Green, 2=Blue, 3=Yellow
    public GameObject simonCanvas; // Canvas con botones del minijuego
    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private int currentStep = 0;
    private int currentPhase = 0;
    private bool playerTurn = false;

    public int maxPhases = 5;
    public int puzzleSimonPoints = 100;
    public int penaltyPoints = 10;

    public int puzzlePoints = 50;
    public GameObject crankObject;
    private bool alreadyUsed = false;
    private string equippedItemID;

    public MonoBehaviour cameraController;
    public MonoBehaviour playerMovement;

    public GameObject diaryPage3;

    private Color[] originalColors;

    void Start()
    {
        originalColors = new Color[colorButtons.Length];
        for (int i = 0; i < colorButtons.Length; i++)
        {
            originalColors[i] = colorButtons[i].GetComponent<Image>().color;
        }
    }
    public void TryInteract()
    {
        if (alreadyUsed) return;

        if (!InventoryManager.Instance.HasItemEquipped())
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("Necesitas una manivela para usar el pozo.");
            return;
        }

        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();
        equippedItemID = InventoryManager.Instance.GetEquippedItemID();
        

        if (equippedItemID == "Manivela") // o usa ReferenceEquals si es el mismo prefab
        {
            
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("¡Has activado el pozo!");
            FindFirstObjectByType<PlayerLogic>().AddPoints(puzzlePoints);

            crankObject.SetActive(true);

            cameraController.enabled = false;
            playerMovement.enabled = false;
            simonCanvas.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            sequence.Clear();
            currentPhase = 0;
            puzzlePoints = 100;
            StartCoroutine(PlaySimonSequence());

            /*if (wellAnimator != null)
                wellAnimator.SetTrigger("Activate");*/

            InventoryManager.Instance.UnequipItem();
            alreadyUsed = true;
        }
        else
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("Ese objeto no puede usarse en el pozo.");
        }
    }
    IEnumerator PlaySimonSequence()
    {
        playerTurn = false;
        playerInput.Clear();
        currentStep = 0;

        if (sequence.Count == 0)
        {
            // Inicia con dos colores aleatorios
            sequence.Add(Random.Range(0, colorButtons.Length));
            sequence.Add(Random.Range(0, colorButtons.Length));
        }
        else
        {
            // Añade solo uno más en las siguientes rondas
            sequence.Add(Random.Range(0, colorButtons.Length));
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < sequence.Count; i++)
        {
            int index = sequence[i];
            HighlightButton(index);
            yield return new WaitForSeconds(0.6f);
            //UnhighlightButton(index);
            yield return new WaitForSeconds(0.3f);
        }

        playerTurn = true;
    }

    public void OnButtonPressed(int buttonIndex)
    {
        if (!playerTurn) return;

        playerInput.Add(buttonIndex);

        if (playerInput[currentStep] == sequence[currentStep])
        {
            currentStep++;
            if (currentStep >= sequence.Count)
            {
                playerTurn = false;
                currentPhase++;

                if (currentPhase >= maxPhases)
                {
                    FindFirstObjectByType<PlayerLogic>().ShowAdvice("¡Puzzle completado!");
                    FindFirstObjectByType<PlayerLogic>().AddPoints(puzzlePoints);
                    simonCanvas.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    cameraController.enabled = true;
                    playerMovement.enabled = true;
                    diaryPage3.SetActive(true);
                }
                else
                {
                    StartCoroutine(PlaySimonSequence());
                }
            }
        }
        else
        {
            // Fallo
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("¡Fallaste! Empezamos de nuevo.");
            puzzlePoints = Mathf.Max(0, puzzlePoints - penaltyPoints);
            sequence.Clear();
            currentPhase = 0;
            StartCoroutine(PlaySimonSequence());
        }
    }

    void HighlightButton(int index)
    {
        Image img = colorButtons[index].GetComponent<Image>();
        StartCoroutine(FlashButton(img, originalColors[index]));
    }

    /*void UnhighlightButton(int index)
    {
        colorButtons[index].GetComponent<Image>().color /= 1.5f;
    }*/

    IEnumerator FlashButton(Image buttonImage, Color originalColor)
    {
        Color highlightColor = Color.Lerp(originalColor, Color.white, 0.5f); // Mezcla el original con blanco (más brillante)
        buttonImage.color = highlightColor;
        yield return new WaitForSeconds(0.4f);
        buttonImage.color = originalColor;
    }
}
