using UnityEngine;

[CreateAssetMenu(fileName = "Potion_", menuName = "Pociones/Potion Data")]
public class PotionData : ScriptableObject
{
    [SerializeField] private string potionName;

    [Header("Targets")]
    [SerializeField] private int dulzura;
    [SerializeField] private int energia;
    [SerializeField] private int frescura;
    [SerializeField] private int intensidad;

    [Header("Recompensas")]
    [SerializeField] private int goldPerfecta = 10;
    [SerializeField] private int goldExitosa = 5;

    public string PotionName => potionName;
    public int Dulzura => dulzura;
    public int Energia => energia;
    public int Frescura => frescura;
    public int Intensidad => intensidad;
    public int GoldPerfecta => goldPerfecta;
    public int GoldExitosa => goldExitosa;
}