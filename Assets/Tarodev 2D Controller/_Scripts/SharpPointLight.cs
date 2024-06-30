using UnityEngine;

[RequireComponent(typeof(Light))]
public class SharpPointLight : MonoBehaviour
{
    public Color lightColor = Color.white;
    public float lightIntensity = 5.0f;
    public float lightRange = 10.0f;

    private Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();
        ConfigureLight();
    }

    void ConfigureLight()
    {
        if (pointLight.type == LightType.Point)
        {
            pointLight.color = lightColor;  // Imposta il colore della luce
            pointLight.intensity = lightIntensity;  // Imposta l'intensità della luce
            pointLight.range = lightRange;  // Imposta il range della luce
            // pointLight.shadows = LightShadows.Hard;  // Imposta le ombre dure
            pointLight.shadowResolution = UnityEngine.Rendering.LightShadowResolution.VeryHigh;  // Alta risoluzione per le ombre
            pointLight.shadowBias = 0.05f;  // Regola il bias delle ombre per evitare sfocature

            // Se necessario, puoi impostare un Light Cookie qui
            // pointLight.cookie = yourLightCookieTexture;
        }
    }

    
}
