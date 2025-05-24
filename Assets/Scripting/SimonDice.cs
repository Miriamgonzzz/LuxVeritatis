using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonDice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button[] colorButtons; // 0=Red, 1=Green, 2=Blue, 3=Yellow
    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private int currentStep = 0;
    private bool playerTurn = false;
    void Start()
    {
        StartCoroutine(PlaySimonSequence());
    }

    IEnumerator PlaySimonSequence()
    {
        playerTurn = false;
        playerInput.Clear();
        sequence.Add(Random.Range(0, colorButtons.Length));

        for (int i = 0; i < sequence.Count; i++)
        {
            int index = sequence[i];
            HighlightButton(index);
            yield return new WaitForSeconds(1f);
            UnhighlightButton(index);
            yield return new WaitForSeconds(0.5f);
        }

        playerTurn = true;
        currentStep = 0;
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
                StartCoroutine(PlaySimonSequence());
            }
        }
        else
        {
            Debug.Log("¡Fallaste!");
            // Aquí puedes reiniciar o mostrar un mensaje.
        }
    }

    void HighlightButton(int index)
    {
        colorButtons[index].GetComponent<Image>().color *= 1.5f; // Brilla
    }

    void UnhighlightButton(int index)
    {
        colorButtons[index].GetComponent<Image>().color /= 1.5f;
    }
}
