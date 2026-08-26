using TMPro;
using UnityEngine;

public enum BrewResult
{
    Perfecta,
    Exitosa,
    Fallida
}

public class PotionMixer : MonoBehaviour
{
    [SerializeField] private int dulzura;
    [SerializeField] private int energia;
    [SerializeField] private int frescura;
    [SerializeField] private int intensidad;

    [Header("Poción actual")]
    [SerializeField] private PotionData currentPotion;

    [Header("Target actual")]
    [SerializeField] private int targetDulzura = 2;
    [SerializeField] private int targetEnergia = 2;
    [SerializeField] private int targetFrescura = 2;
    [SerializeField] private int targetIntensidad = 2;

    [Header("Textos actuales")]
    [SerializeField] private TMP_Text txtActualDulzura;
    [SerializeField] private TMP_Text txtActualEnergia;
    [SerializeField] private TMP_Text txtActualFrescura;
    [SerializeField] private TMP_Text txtActualIntensidad;

    [Header("Textos target")]
    [SerializeField] private TMP_Text txtTargetDulzura;
    [SerializeField] private TMP_Text txtTargetEnergia;
    [SerializeField] private TMP_Text txtTargetFrescura;
    [SerializeField] private TMP_Text txtTargetIntensidad;

    private void Start()
    {
        UpdateStatsUI();

        if (currentPotion != null)
            LoadPotion(currentPotion);
        else
            UpdateTargetUI();
    }

    public void LoadPotion(PotionData potion)
    {
        currentPotion = potion;
        targetDulzura = potion.Dulzura;
        targetEnergia = potion.Energia;
        targetFrescura = potion.Frescura;
        targetIntensidad = potion.Intensidad;

        UpdateTargetUI();
    }

    public void AddIngredient(IngredientStats ingredient)
    {
        dulzura += ingredient.Dulzura;
        energia += ingredient.Energia;
        frescura += ingredient.Frescura;
        intensidad += ingredient.Intensidad;

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (txtActualDulzura != null)
            txtActualDulzura.text = dulzura.ToString();
        if (txtActualEnergia != null)
            txtActualEnergia.text = energia.ToString();
        if (txtActualFrescura != null)
            txtActualFrescura.text = frescura.ToString();
        if (txtActualIntensidad != null)
            txtActualIntensidad.text = intensidad.ToString();
    }

    private void UpdateTargetUI()
    {
        if (txtTargetDulzura != null)
            txtTargetDulzura.text = targetDulzura.ToString();
        if (txtTargetEnergia != null)
            txtTargetEnergia.text = targetEnergia.ToString();
        if (txtTargetFrescura != null)
            txtTargetFrescura.text = targetFrescura.ToString();
        if (txtTargetIntensidad != null)
            txtTargetIntensidad.text = targetIntensidad.ToString();
    }

    public void ResetMix()
    {
        dulzura = 0;
        energia = 0;
        frescura = 0;
        intensidad = 0;

        UpdateStatsUI();
    }

    public BrewResult Brew()
    {
        bool perfecta =
            dulzura == targetDulzura &&
            energia == targetEnergia &&
            frescura == targetFrescura &&
            intensidad == targetIntensidad;

        bool exitosa =
            Mathf.Abs(dulzura - targetDulzura) <= 1 &&
            Mathf.Abs(energia - targetEnergia) <= 1 &&
            Mathf.Abs(frescura - targetFrescura) <= 1 &&
            Mathf.Abs(intensidad - targetIntensidad) <= 1;

        if (perfecta)
            return BrewResult.Perfecta;

        if (exitosa)
            return BrewResult.Exitosa;

        return BrewResult.Fallida;
    }
}
