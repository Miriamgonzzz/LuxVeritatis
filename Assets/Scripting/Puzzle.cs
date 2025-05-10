using UnityEngine;

//clase padre Puzzle para que hereden los puzzles del juego
public abstract class Puzzle : MonoBehaviour
{
    public bool isSolved = false;

    private void OnMouseDown()
    {
        if (!isSolved)
        {
            StartPuzzleLogic();
        }
    }

    protected abstract void StartPuzzleLogic();

    public virtual void PuzzleSolved()
    {
        isSolved = true;
        Debug.Log("¡Puzzle resuelto!");
    }
}
