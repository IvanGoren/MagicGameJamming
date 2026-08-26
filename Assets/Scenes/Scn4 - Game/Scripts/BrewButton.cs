using UnityEngine;

public class BrewButton : MonoBehaviour
{
    [SerializeField] private CustomerManager customerManager;

    private void OnMouseDown()
    {
        customerManager.BrewCurrentPotion();
    }
}
