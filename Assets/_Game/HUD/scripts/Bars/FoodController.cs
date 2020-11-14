using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodController : MonoBehaviour
{
    #region  object bar
    
    public GameObject foodBar;     // obj da barra de comida 

    #endregion

    #region references

    private Image imageFoodBar;     // referencia imagem a ser adquirida para usar o fill amount

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        imageFoodBar = foodBar.GetComponent<Image>();   
        StartCoroutine(TimeForFoodDigest());
    }

    #region method for FoodCheck

    private int minutos = 60;    // um minuto vale sessenta segundos
    [SerializeField]private float minutoQuantity;   // quantidade de minutos para barra reduzir um determinador valor
    [SerializeField]private float indiceIncreased;  // indice de acrescimo no tempo, toda vez que ela for chamada o tempo aumentará
    [SerializeField]private float indiceIncreaseTimeMax;  // indice maximo de acrescimo
    [SerializeField]private float reductionValue;   // indice de redução da barra de comida

    IEnumerator TimeForFoodDigest()        // função da digestão, isto é da redução da barra de comida
    {
        yield return new WaitForSeconds(minutos * (minutoQuantity + indiceIncreased));
        imageFoodBar.fillAmount -= reductionValue;

        if(indiceIncreased >= indiceIncreaseTimeMax)   // limite para nao permitir que o tempo aumente demais
        {
            indiceIncreased = 0.5f;
        }

        StartCoroutine(TimeForFoodDigest());
        StartCoroutine(TimeForIncreasedIndice());
    }

    IEnumerator TimeForIncreasedIndice()
    {
        yield return new WaitForSeconds(1);
        indiceIncreased += 0.01f;
    }

    #endregion
}
