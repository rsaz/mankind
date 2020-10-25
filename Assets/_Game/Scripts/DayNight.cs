using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNight : MonoBehaviour
{
    [SerializeField] private Light2D sun;
    [SerializeField] private float intensityFactor = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Sunset());
    }

    IEnumerator Sunset()
    {
        // to define a day and night cycle time
        yield return new WaitForSeconds(5f);

        while (sun.intensity > 0.5f)
        {
            sun.intensity -= intensityFactor;
            yield return new WaitForSeconds(intensityFactor);
        }
       
        StartCoroutine(Sunrise());
        yield return new WaitForSeconds(5f);
        StartCoroutine(Sunset());
    }

    IEnumerator Sunrise()
    {
        while (sun.intensity < 1f)
        {
            sun.intensity += intensityFactor;
            yield return new WaitForSeconds(intensityFactor);
        }
        StartCoroutine(Sunset());
        yield return new WaitForSeconds(5f);
        StartCoroutine(Sunrise());
    }
}
