using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnergyController : MonoBehaviour
{
    #region Bars Object
    //referencia do gameobject da barra de energia
    public GameObject energyBar;

    #endregion

    #region references
    // componente image a ser utilizado por meio do fill amount
    private Image imageEnergyBar;
    
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        imageEnergyBar = energyBar.GetComponent<Image>();
        StartCoroutine(TimeForEnergyDecrease());
    }

    #region  method for energy decrease 
    
    private int minuto = 60; // um  minuto vale sessenta segundos
    [SerializeField]private float minutoQuantity;  // quantidade de minutos
    [SerializeField]private float indiceIncreaseTime;   // indice de acrescimo ao tempo
    [SerializeField]private float indiceIncreaseTimeMax;  // indice maximo de acrescimo
    [SerializeField]private float reductionBar;    // fator de redução do fillAmount da barra

    IEnumerator TimeForEnergyDecrease()
    {
        yield return new WaitForSeconds(minuto * ( minutoQuantity + indiceIncreaseTime));
        imageEnergyBar.fillAmount -= reductionBar;

        if(indiceIncreaseTime >= indiceIncreaseTimeMax)  // limite para nao permitir que o tempo aumente demais
        {
            indiceIncreaseTime = 0.5f;
        }

        StartCoroutine(TimeForEnergyDecrease());
        StartCoroutine(TimeForIndiceIncrease());
    }

    IEnumerator TimeForIndiceIncrease()
    {
        yield return new WaitForSeconds(1);
        indiceIncreaseTime += 0.01f;
    }

    #endregion
}
