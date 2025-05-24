using UnityEngine;

[RequireComponent(typeof(Light))]
public class DiaryLightPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;      //velocidad del parpadeo de la luz
    public float pulseAmount = 3f;   //cu�nto cambia la intensidad de la luz
    private Light pulseLight;
    private float baseIntensity;

    void Start()
    {
        pulseLight = GetComponent<Light>();
        baseIntensity = pulseLight.intensity; //coge la intensidad base de la luz a�adida al objeto
    }

    void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        pulseLight.intensity = baseIntensity + pulse;
    }
}
