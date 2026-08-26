using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private float dayDuration = 360f;
    [SerializeField] private int gold;
    [SerializeField] private int perfectPotions;
    [SerializeField] private int successfulPotions;
    [SerializeField] private int failedPotions;
    [SerializeField] private TMP_Text txtGold;

    public int Gold => gold;
    public int PerfectPotions => perfectPotions;
    public int SuccessfulPotions => successfulPotions;
    public int FailedPotions => failedPotions;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI();
        StartCoroutine(DayTimer());
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();

        Debug.Log("Oro obtenido: " + amount + " | Oro total: " + gold);
    }
    
    public void RegisterPotionResult(BrewResult result)
    {
        if (result == BrewResult.Perfecta)
        {
            perfectPotions++;
        }
        else if (result == BrewResult.Exitosa)
        {
            successfulPotions++;
        }
        else
        {
            failedPotions++;
        }
    }
    
    private IEnumerator DayTimer()
    {
        yield return new WaitForSeconds(dayDuration);

        customerManager.EndDay();
        SceneManager.LoadScene("Scn5 - PostGame");
    }
    private void UpdateGoldUI()
{
    txtGold.text = gold.ToString();
}
}