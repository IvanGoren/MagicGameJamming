using System.Collections;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private PotionMixer potionMixer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PotionData[] availablePotions;
    [SerializeField] private float nextCustomerDelay = 5f;
    [SerializeField] private float customerWaitTime = 30f;
    [Header("Clientes")]
    [SerializeField] private CustomerData[] availableCustomers;
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private AudioSource customerAudioSource; 

    private int lastPotionIndex = -1;
    private bool isChangingCustomer;
    private bool dayIsOver;
    private Coroutine customerTimer;
    private PotionData currentPotion;
    private CustomerData currentCustomer;

    private void Start()
    {
        LoadRandomCustomer();
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

        ShowCustomerReaction(result);
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
        LoadRandomCustomer();
        LoadRandomPotion();

        isChangingCustomer = false;
    }

    private void LoadRandomCustomer()
    {
        if (dayIsOver || availableCustomers == null || availableCustomers.Length == 0)
            return;

        int randomIndex = Random.Range(0, availableCustomers.Length);
        currentCustomer = availableCustomers[randomIndex];

        customerSpriteRenderer.sprite = currentCustomer.idleSprite;
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
        ShowRequestDialogue();

        StopCustomerTimer();
        customerTimer = StartCoroutine(CustomerTimer());
    }

    private void ShowRequestDialogue()
    {
        if (currentCustomer == null || currentCustomer.potionDialogues == null)
            return;

        foreach (CustomerPotionDialogue dialogue in currentCustomer.potionDialogues)
        {
            if (dialogue.potion == currentPotion)
            {
                dialogueText.text = dialogue.requestText;
                return;
            }
        }

        dialogueText.text = "";
        Debug.LogWarning(
            "No se encontró diálogo para " +
            currentCustomer.customerName +
            " y la poción " +
            currentPotion.name
        );
    }
    private void ShowCustomerReaction(BrewResult result)
    {
        if (currentCustomer == null || currentCustomer.potionDialogues == null)
            return;

        CustomerPotionDialogue currentDialogue = null;

        foreach (CustomerPotionDialogue dialogue in currentCustomer.potionDialogues)
        {
            if (dialogue.potion == currentPotion)
            {
                currentDialogue = dialogue;
                break;
            }
        }

        if (currentDialogue == null)
            return;

        switch (result)
        {
            case BrewResult.Perfecta:
                customerSpriteRenderer.sprite = currentCustomer.perfectSprite;
                dialogueText.text = currentDialogue.perfectText;

                if (currentCustomer.perfectSFX != null)
                    customerAudioSource.PlayOneShot(currentCustomer.perfectSFX);

                break;

            case BrewResult.Exitosa:
                customerSpriteRenderer.sprite = currentCustomer.successfulSprite;
                dialogueText.text = currentDialogue.successfulText;

                if (currentCustomer.successfulSFX != null)
                    customerAudioSource.PlayOneShot(currentCustomer.successfulSFX);

                break;

            case BrewResult.Fallida:
                customerSpriteRenderer.sprite = currentCustomer.failedSprite;
                dialogueText.text = currentDialogue.failedText;

                if (currentCustomer.failedSFX != null)
                    customerAudioSource.PlayOneShot(currentCustomer.failedSFX);

                break;
        }
    }
    private IEnumerator CustomerTimer()
    {
        yield return new WaitForSeconds(customerWaitTime);

        customerTimer = null;
        Debug.Log("El cliente se fue por falta de tiempo.");
        gameManager.RegisterPotionResult(BrewResult.Fallida);

        ShowCustomerReaction(BrewResult.Fallida);
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
