using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItens : MonoBehaviour
{
    #region  object bars
    // objetos das barras
    public GameObject healthBar;
    public GameObject foodBar;
    public GameObject waterBar;
    public GameObject energyBar; 

    #endregion

    #region references
    // referencia da imagem a ser usada o fillAmount
    private Image imageHealthBar;
    private Image imageFoodBar;
    private Image imageWaterBar;
    private Image imageEnergyBar;
    
    #endregion

    #region number to increase bars
    // numeros de acrescimos no fillAmount das barras
    [SerializeField] private float pizzaIncrease;
    [SerializeField] private float hotDogIncrease;
    [SerializeField] private float waterBotlleIncrease;
    [SerializeField] private float energeticIncrease;
    [SerializeField] private float kitMedicIncrease;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        imageEnergyBar = energyBar.GetComponent<Image>();
        imageFoodBar = foodBar.GetComponent<Image>();
        imageHealthBar = healthBar.GetComponent<Image>();
        imageWaterBar = waterBar.GetComponent<Image>();
    }

    #region buttons for Itens 

    // botoes para se alocar nos itens do inventario

    public void pizza()
    {
        imageFoodBar.fillAmount += pizzaIncrease;
    }

    public void HotDog()
    {
        imageFoodBar.fillAmount += hotDogIncrease;
    }

    public void waterBotlle()
    {
        imageWaterBar.fillAmount += waterBotlleIncrease;
    }

    public void Energetic()
    {
        imageEnergyBar.fillAmount += energeticIncrease;
    }

    public void KitMedic()
    {
        imageHealthBar.fillAmount += kitMedicIncrease;
    }

    #endregion

}
