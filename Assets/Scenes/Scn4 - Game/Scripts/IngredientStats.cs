using UnityEngine;

public class IngredientStats : MonoBehaviour
{
    [SerializeField] private string ingredientName;

    [SerializeField] private int dulzura;
    [SerializeField] private int energia;
    [SerializeField] private int frescura;
    [SerializeField] private int intensidad;

    public string IngredientName => ingredientName;
    public int Dulzura => dulzura;
    public int Energia => energia;
    public int Frescura => frescura;
    public int Intensidad => intensidad;
}