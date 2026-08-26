using System.Collections;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private PotionMixer potionMixer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PotionData[] availablePotions;
    [SerializeField] private float nextCustomerDelay = 5f;
    [SerializeField] private float customerWaitTime = 30f;

    private int lastPotionIndex = -1;
    private bool isChangingCustomer;
    private bool dayIsOver;
    private Coroutine customerTimer;
    private PotionData currentPotion;

    private void Start()
    {
        LoadRandomPotion();
    }

    public void BrewCurrentPotion()
{
    if (isChangingCustomer || dayIsOver)
        return;

    BrewResult result = potionMixer.Brew();
    gameManager.RegisterPotionResult(result);

    if (result == BrewResult.Perfecta)
    {
        gameManager.AddGold(currentPotion.GoldPerfecta);
    }
    else if (result == BrewResult.Exitosa)
    {
        gameManager.AddGold(currentPotion.GoldExitosa);
    }

    Debug.Log("Resultado de la pócima: " + result);

    EndCurrentCustomer();
}

    private void EndCurrentCustomer()
    {
        if (isChangingCustomer)
            return;

        isChangingCustomer = true;
        StopCustomerTimer();
        StartCoroutine(LoadNextPotionAfterDelay());
    }

    private IEnumerator LoadNextPotionAfterDelay()
    {
        yield return new WaitForSeconds(nextCustomerDelay);

        potionMixer.ResetMix();
        LoadRandomPotion();

        isChangingCustomer = false;
    }

    private void LoadRandomPotion()
    {
        if (dayIsOver || availablePotions == null || availablePotions.Length == 0)
            return;

        int randomIndex;

        if (availablePotions.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            do
            {
                randomIndex = Random.Range(0, availablePotions.Length);
            }
            while (randomIndex == lastPotionIndex);
        }

        lastPotionIndex = randomIndex;
        currentPotion = availablePotions[randomIndex];
        potionMixer.LoadPotion(currentPotion);

        StopCustomerTimer();
        customerTimer = StartCoroutine(CustomerTimer());
    }

    private IEnumerator CustomerTimer()
    {
        yield return new WaitForSeconds(customerWaitTime);

        customerTimer = null;
        Debug.Log("El cliente se fue por falta de tiempo.");
        gameManager.RegisterPotionResult(BrewResult.Fallida);

        EndCurrentCustomer();
    }

    private void StopCustomerTimer()
    {
        if (customerTimer != null)
        {
            StopCoroutine(customerTimer);
            customerTimer = null;
        }
    }

    public void EndDay()
    {
        dayIsOver = true;
        StopAllCoroutines();

        Debug.Log("Fin de la jornada.");
    }
}