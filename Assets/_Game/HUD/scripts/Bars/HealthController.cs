using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthController : MonoBehaviour
{
    #region Object Bar
    //gameObject das imageSlider de cada barra 
    public GameObject healthBar;   
    public GameObject foodBar;      
    public GameObject waterBar;

    #endregion

    #region  referencias
    // referencia da imagem para utilizar o fill Amount
    private Image healthImageBar;
    private Image foodImageBar;
    private Image waterImageBar; 

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        healthImageBar = healthBar.GetComponent<Image>();
        foodImageBar = foodBar.GetComponent<Image>();
        waterImageBar = waterBar.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarController();
    }

    #region method for Health Controller
    
    private void HealthBarController()
    {  
        // verifica as barras de comida e agua, se estiverem vazias a vida diminui
       // se estiverem cheias ela aumenta
        
        if(foodImageBar.fillAmount == 0)
        {
            healthImageBar.fillAmount -= 0.2f * Time.deltaTime;
        }
        else if(foodImageBar.fillAmount == 1)
        {
            healthImageBar.fillAmount += 0.3f * Time.deltaTime;
        }
        if(waterImageBar.fillAmount == 0)
        {
            healthImageBar.fillAmount -= 0.3f * Time.deltaTime;
        }
        else if(waterImageBar.fillAmount == 1)
        {
            healthImageBar.fillAmount += 0.2f * Time.deltaTime;
        }

        // verifica a vida para efetuar a ação de player death
        if(healthImageBar.fillAmount == 0)
        {
            print("you die");
        }
    }

    #endregion
}
