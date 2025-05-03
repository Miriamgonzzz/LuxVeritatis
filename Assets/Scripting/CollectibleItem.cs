using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Item")]
public class CollectibleItem : ScriptableObject
{
    public string ID;
    public Sprite icon;
    public GameObject prefabToInspect;

    [HideInInspector] public string itemName;
    [HideInInspector] public string description;
    [HideInInspector] public string storyText;
}
