using UnityEngine;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private PotionMixer potionMixer;

    private void OnMouseDown()
    {
        potionMixer.ResetMix();
    }
}