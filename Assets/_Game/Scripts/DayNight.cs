using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNight : MonoBehaviour
{
    [SerializeField] private Light2D sun;
    
    [SerializeField] private float dayDurationInMinutes= 1;
    [SerializeField] private float nightDurationInMinutes = 0.5f;

    [SerializeField] private float totalDarknnesStayTime = 1f;
    [SerializeField] private float fullBrightnessStayTime = 1f;

    [Range(0, 1)]
    [SerializeField] private float maxDayLightIntensity = 1.0f;
    [Range(0, 1)]
    [SerializeField] private float maxNightLightIntensity = 0.5f;

    private float dayIntensityFactor;
    private float nightIntensityFactor;




    // Start is called before the first frame update
    void Start()
    {
        dayIntensityFactor = (maxDayLightIntensity - maxNightLightIntensity) / (dayDurationInMinutes * 6000);
        nightIntensityFactor = (maxDayLightIntensity - maxNightLightIntensity) / (nightDurationInMinutes * 6000);
        StartCoroutine(Sunset());
        
    }

    IEnumerator Sunset()
    {
        // to define a day and night cycle time
        yield return new WaitForSeconds(fullBrightnessStayTime * 60);

        while (sun.intensity > maxNightLightIntensity)
        {
            
            sun.intensity -= dayIntensityFactor;
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
            yield return new WaitForSecondsRealtime(0.01f);
        }
        StartCoroutine(Sunset());
        

    }
}
