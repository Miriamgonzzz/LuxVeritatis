using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Item")]
public class CollectibleItem : ScriptableObject
{
    public string ID;
    public Sprite icon;
    public GameObject prefabToInspect;

    [Header("Datos adicionales")]
    public string itemName;        // T�tulo de la p�gina
    public string itemType;        // Debe ser "textItem" para que funcione como p�gina del diario
    public string description;     // Descripci�n breve
    [TextArea(4, 10)] public string storyText; // Texto completo de la p�gina

}
