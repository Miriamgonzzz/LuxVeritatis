using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Item")]
public class CollectibleItem : ScriptableObject
{
    public string ID;
    public Sprite icon;
    public GameObject prefabToInspect;

    [Header("Datos adicionales")]
    public string itemName;        // Título de la página
    public string itemType;        // Debe ser "textItem" para que funcione como página del diario
    public string description;     // Descripción breve
    [TextArea(4, 10)] public string storyText; // Texto completo de la página

}
