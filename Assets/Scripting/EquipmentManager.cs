using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    private string equippedItemID;

    //método Singleton para crear una sola instancia del EquipmentManager
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EquipItem(string itemID)
    {
        equippedItemID = itemID;
        Debug.Log($"Objeto equipado: {itemID}");
    }

    public void UnequipItem()
    {
        equippedItemID = null;
    }

    public string GetEquippedItemID()
    {
        return equippedItemID;
    }

    public bool HasItemEquipped()
    {
        return !string.IsNullOrEmpty(equippedItemID);
    }
}
