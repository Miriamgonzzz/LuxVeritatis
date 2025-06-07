using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoSpawner : MonoBehaviour
{
    public Sprite[] photoSprites;             // Asigna aquí tus fotos
    public GameObject photoPrefab;            // El prefab base
    public RectTransform spawnArea;           // El área donde caen (Canvas o panel)
    public float spawnInterval = 0.5f;        // Tiempo entre fotos

    private List<Sprite> remainingSprites = new List<Sprite>(); // Para controlar no repetir

    void Start()
    {
        remainingSprites.AddRange(photoSprites);
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(10f); // Esperar 10 segundos antes de empezar
        StartCoroutine(SpawnPhotos());
    }

    IEnumerator SpawnPhotos()
    {
        while (remainingSprites.Count > 0)
        {
            SpawnPhoto();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPhoto()
    {
        // Elegimos un sprite aleatorio y lo eliminamos de la lista
        int index = Random.Range(0, remainingSprites.Count);
        Sprite chosenSprite = remainingSprites[index];
        remainingSprites.RemoveAt(index);

        // Instanciamos el prefab
        GameObject photo = Instantiate(photoPrefab, spawnArea);
        Image img = photo.GetComponent<Image>();
        img.sprite = chosenSprite;

        RectTransform rt = photo.GetComponent<RectTransform>();

        // Obtener el tamaño real de la imagen (para evitar que se corte)
        float photoWidth = rt.sizeDelta.x;
        float photoHeight = rt.sizeDelta.y;

        // Cálculo de posición aleatoria dentro del área visible
        float x = Random.Range(
            -spawnArea.rect.width / 2 + photoWidth / 2,
            spawnArea.rect.width / 2 - photoWidth / 2
        );
        float y = Random.Range(
            -spawnArea.rect.height / 2 + photoHeight / 2,
            spawnArea.rect.height / 2 - photoHeight / 2
        );

        rt.anchoredPosition = new Vector2(x, y);

        // Rotación y escala opcionales
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
        rt.localScale = Vector3.one * Random.Range(0.9f, 1.1f);

        // Fade-in
        StartCoroutine(FadeIn(photo.GetComponent<CanvasGroup>()));
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        float duration = 1f;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
