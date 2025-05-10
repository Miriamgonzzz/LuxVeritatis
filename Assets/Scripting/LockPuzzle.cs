using UnityEngine;

public class LockPuzzle : Puzzle
{
    [SerializeField] private GameObject lockCanvas; //canvas con la cerradura
    [SerializeField] private string correctKeyId;
    [SerializeField] private GameObject doorToOpen;
    [SerializeField] private GameObject[] keyObjectsInScene; //llaves no recogidas en escena

    protected override void StartPuzzleLogic()
    {
        string equippedItemId = EquipmentManager.Instance.GetEquippedItemID();

        if (string.IsNullOrEmpty(equippedItemId))
        {
            lockCanvas.SetActive(true); // mostrar canvas
        }
        else if (equippedItemId == correctKeyId)
        {
            PuzzleSolved();
        }
        else
        {
            Debug.Log("La llave no es correcta.");
        }
    }

    public override void PuzzleSolved()
    {
        base.PuzzleSolved();

        lockCanvas.SetActive(false);
        doorToOpen.SetActive(false); // o usar animación

        foreach (GameObject key in keyObjectsInScene)
        {
            if (key != null)
                Destroy(key);
        }

        InventoryManager.Instance.RemoveAllItemsOfType("Key"); // Implementar si no existe
        EquipmentManager.Instance.UnequipItem();
    }
}
