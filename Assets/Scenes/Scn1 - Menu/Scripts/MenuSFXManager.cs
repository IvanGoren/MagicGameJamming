using UnityEngine;

public class MenuSFXManager : MonoBehaviour
{
    public static MenuSFXManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("SFX")]
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip confirmSFX;
    [SerializeField] private AudioClip cancelSFX;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayHover()
    {
        audioSource.PlayOneShot(hoverSFX, 0.3f);
    }

    public void PlayConfirm()
    {
        audioSource.PlayOneShot(confirmSFX, 0.2f);
    }

    public void PlayCancel()
    {
        audioSource.PlayOneShot(cancelSFX, 0.2f);
    }
}