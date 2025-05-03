using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTextData
{
    public string ID;
    public string itemName;
    public string description;     
    public string storyText;     
}

public class ItemsDataLoader : MonoBehaviour
{
    public List<CollectibleItem> itemsToUpdate; //la lista de objetos a actualizar. Hay que arrastrarlos todos desde el inspector

    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("itemsInfo");
        if (jsonFile != null)
        {
            //convertimos el JSON en un objeto de C# gracias a la clase Wrapper. El Wrapper es un objeto que contiene ItemTextData, que es el campo items del JSON
            ItemTextData[] textDataArray = JsonUtility.FromJson<Wrapper>(jsonFile.text).items;

            foreach (var itemData in itemsToUpdate)
            {
                foreach (var textData in textDataArray)
                {
                    if (itemData.ID == textData.ID) //si el ID del CollectibleItem coincide con el almacena en el json, le carga la descripción y el texto
                    {
                        itemData.itemName = textData.itemName;
                        itemData.description = textData.description;
                        itemData.storyText = textData.storyText;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No se pudo cargar el archivo JSON de descripciones");
        }
    }

    [System.Serializable]
    private class Wrapper //clase contenedora que se usa para deserializar el JSON en Unity cuando se tiene un array dentro de un objeto.
    {
        public ItemTextData[] items;
    }
}

