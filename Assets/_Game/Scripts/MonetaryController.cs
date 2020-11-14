using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonetaryController : MonoBehaviour
{
    #region Monetary base Minerals em Gewicht
    [SerializeField] public float seienPesoGt = 4.23f;  // liga de nióbio e paladio
    [SerializeField] public float legdPesoGt = 2.39f;    // liga de platina e prata
    [SerializeField] public float blisedPesoGt = 5.79f;   // algo semelhante a um diamante, mas com propriedades do grafeno
    #endregion

    // a proporção de GT para KG é de 4/7

    #region Monetary base Minerals em kilograma
    [SerializeField] public float seienPesoKg = 2.41f;
    [SerializeField] public float legdPesoKg = 1.36f;
    [SerializeField] public float blisedPesoKg = 3.30f;
    #endregion

    #region Anethus(our name money) System
    private float dolar;    // variavel para o dolar
    private float anethus;     // variavel para o dinheiro do jogo
    private float minimumWage;   // variavel para o salario minimo
    private float wageNumber;        // variavel para o numero de salarios minimos
    private void conversion()
    {
        dolar = (seienPesoKg * legdPesoKg * blisedPesoKg) / 3;
        anethus = (((seienPesoGt * legdPesoGt * blisedPesoGt) / 3 * dolar) / 2.35f);
        minimumWage = ((((seienPesoGt * legdPesoGt * blisedPesoGt) / 3) * 800) / dolar);
    }
    #endregion

    #region imposts
    private float stateImpost;      // imposto do estado
    private float alimantationImpost;     // imposto da alimentação
    private float tecnologyImpost;      // imposto da tecnologia
    private float educationImpost;     // imposto para a educação
    private float healthImpost;       // imposto para a saude
    private float ImpostWage;         // impostos sobre o salario minimo
    private void ImpostSystem()      // metodo que define o valor de cada imposto
    {
        stateImpost = 0.01f;
        alimantationImpost = 0.03f;
        tecnologyImpost = 0.04f;
        educationImpost = 0.05f;
        healthImpost = 0.06f;
        ImpostWage = stateImpost + alimantationImpost + tecnologyImpost + healthImpost + educationImpost;
    }
    #endregion

    #region Player Profissions
    private bool desempregado;
    private bool graduado;
    private bool mestre;
    private bool doutor;
    private bool medico;
    private bool advogado;
    private bool professor;
    #endregion

    #region referencias

    public GameObject numberMoney;
    private TextMeshProUGUI textNumberMoney;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        textNumberMoney = numberMoney.GetComponent<TextMeshProUGUI>();
        desempregado = false;
        graduado = false;
        medico = true;

        TimeProfit();
        conversion();
        ImpostSystem();
        PayStateSystem();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region methods profit player
    public void PayStateSystem()
    {
        if (desempregado)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyUnem());

        }
        else if (graduado)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyGrad());
        }
        else if (mestre)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyMaster());
        }
        else if (doutor)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyDoc());
        }
        else if (medico)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyDoctor());
        }
        else if (advogado)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyAdv());
        }
        else if (professor)
        {
            textNumberMoney.text = minimumWage.ToString();
            StartCoroutine(TimeAddMoneyTea());
        }
    }
    #endregion

    #region Add money system base
    private float t;
    private float i = 25;
    private float amountWon;
    [SerializeField] private float minuto = 30;

    private void TimeProfit()
    {
        t = 60 * minuto;       
    }
    IEnumerator TimeUpPorcentage()
    {
        yield return new WaitForSeconds(1);
        i++;
    }
    IEnumerator TimeUpTime()
    {
        yield return new WaitForSeconds(1);
        minuto += 30;
    }

    #endregion

    #region time for add money Desempregado
    IEnumerator TimeAddMoneyUnem()
    {
        wageNumber = 1;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage += amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyUnem());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money Graduado
    IEnumerator TimeAddMoneyGrad()
    {
        wageNumber = 2;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyGrad());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money mestre
    IEnumerator TimeAddMoneyMaster()
    {
        wageNumber = 3;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyMaster());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money doutor
    IEnumerator TimeAddMoneyDoc()
    {
        wageNumber = 4;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyDoc());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money medico
    IEnumerator TimeAddMoneyDoctor()
    {
        wageNumber = 4.5f;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyDoctor());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money advogado
    IEnumerator TimeAddMoneyAdv()
    {
        wageNumber = 3.5f;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyAdv());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region time for add money professor
    IEnumerator TimeAddMoneyTea()
    {
        wageNumber = 5f;
        yield return new WaitForSeconds(t);
        minimumWage = minimumWage * wageNumber;
        amountWon = ((i / 100) * minimumWage) - (minimumWage * ImpostWage);
        minimumWage = minimumWage + amountWon;
        textNumberMoney.text = minimumWage.ToString();

        if (i < 50)
        {
            StartCoroutine(TimeUpPorcentage());
        }
        else if (i == 50)
        {
            StopCoroutine(TimeUpPorcentage());
        }

        StartCoroutine(TimeAddMoneyTea());
        StartCoroutine(TimeUpTime());

    }
    #endregion

    #region buttons
    public void Graduate()
    {
        graduado = true;
        PayStateSystem();
    }
    public void Master()
    {
        mestre = true;
        PayStateSystem();
    }
    public void Doctor()
    {
        doutor = true;
        PayStateSystem();
    }
    public void Medico()
    {
        medico = true;
        PayStateSystem();
    }
    public void Lawyer()
    {
        advogado = true;
        PayStateSystem();
    }
    public void Teacher()
    {
        professor = true;
        PayStateSystem();
    }
    #endregion
}