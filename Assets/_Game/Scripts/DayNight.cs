using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNight : MonoBehaviour
{
    [SerializeField] private Light2D sun;
    
    [SerializeField] private float dayDurationInMinutes= 1;
    [SerializeField] private float nightDurationInMinutes = 0.5f;
    private float dayDurationMilliseconds;
    private float nightDurationMilliseconds;

    [SerializeField] private float totalDarknnesStayTime = 0.1f;
    [SerializeField] private float fullBrightnessStayTime = 0.1f;

    [Range(0, 1)]
    [SerializeField] private float maxDayLightIntensity = 1.0f;
    [Range(0, 1)]
    [SerializeField] private float maxNightLightIntensity = 0.5f;

    [SerializeField] private Color afternoonColor = new Color(255, 180, 69);
    [SerializeField] private Color nightColor = new Color(49,41,103);

    private float dayIntensityFactor;
    private float nightIntensityFactor;

    private float dayColorIntensityFactor;
    private float nightColorIntensityFactor;


    // Start is called before the first frame update
    void Start()
    {
        dayDurationMilliseconds = dayDurationInMinutes * 6000;
        nightDurationMilliseconds = nightDurationInMinutes * 6000;
        dayColorIntensityFactor = dayDurationMilliseconds / 10000000;
        nightColorIntensityFactor = nightDurationMilliseconds / 10000000;
        dayIntensityFactor = (maxDayLightIntensity - maxNightLightIntensity) / dayDurationMilliseconds;
        nightIntensityFactor = (maxDayLightIntensity - maxNightLightIntensity) / dayDurationMilliseconds;
        StartCoroutine(Sunset());
        
    }

    IEnumerator Sunset()
    {
        // to define a day and night cycle time
        yield return new WaitForSeconds(fullBrightnessStayTime * 60);

        while (sun.intensity > maxNightLightIntensity)
        {
            
            sun.intensity -= dayIntensityFactor;
            sun.color = Color.Lerp(sun.color, nightColor, dayColorIntensityFactor );
            yield return new WaitForSeconds(0.01f);
        }
        
        StartCoroutine(Sunrise());
    }

    IEnumerator Sunrise()
    {
        yield return new WaitForSeconds(totalDarknnesStayTime * 60);
        while (sun.intensity < 1f)
        {
            sun.intensity += nightIntensityFactor;
            sun.color = Color.Lerp(sun.color, afternoonColor, nightColorIntensityFactor);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        StartCoroutine(Sunset());
        

    }
}
