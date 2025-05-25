using UnityEngine;

public class WellInteraction : MonoBehaviour
{
    //public Animator wellAnimator;
    public int puzzlePoints = 50;
    public GameObject crankPrefab; // Asignar el prefab correcto en el inspector
    public GameObject crankObject;
    private bool alreadyUsed = false;

    public void TryActivateWell()
    {
        if (alreadyUsed) return;

        if (!InventoryManager.Instance.HasItemEquipped())
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("Necesitas una manivela para usar el pozo.");
            return;
        }

        GameObject equippedObject = InventoryManager.Instance.GetEquippedObject();

        if (equippedObject.name == crankPrefab.name) // o usa ReferenceEquals si es el mismo prefab
        {
            FindFirstObjectByType<PlayerLogic>().ShowAdvice("¡Has activado el pozo!");
            FindFirstObjectByType<PlayerLogic>().AddPoints(puzzlePoints);

            crankObject.SetActive(true);

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
}
