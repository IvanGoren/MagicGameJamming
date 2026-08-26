using TMPro;
using UnityEngine;

public class PostGameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text txtOroRecolectado;
    [SerializeField] private TMP_Text txtOroAcumulado;
    [SerializeField] private TMP_Text txtPerfectas;
    [SerializeField] private TMP_Text txtExitosas;
    [SerializeField] private TMP_Text txtFallidas;

    private void Start()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return;

        txtOroRecolectado.text = gameManager.Gold.ToString();
        txtOroAcumulado.text = gameManager.Gold.ToString();
        txtPerfectas.text = gameManager.PerfectPotions.ToString();
        txtExitosas.text = gameManager.SuccessfulPotions.ToString();
        txtFallidas.text = gameManager.FailedPotions.ToString();
    }
}