using UnityEngine;

[System.Serializable]
public class CustomerPotionDialogue
{
    public PotionData potion;

    [TextArea(2, 5)]
    public string requestText;

    [TextArea(2, 5)]
    public string perfectText;

    [TextArea(2, 5)]
    public string successfulText;

    [TextArea(2, 5)]
    public string failedText;
}

[CreateAssetMenu(
    fileName = "NewCustomer",
    menuName = "Clientes/Customer Data"
)]
public class CustomerData : ScriptableObject
{
    [Header("Identidad")]
    public string customerName;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite perfectSprite;
    public Sprite successfulSprite;
    public Sprite failedSprite;

    [Header("Audios")]
    public AudioClip perfectSFX;
    public AudioClip successfulSFX;
    public AudioClip failedSFX;

    [Header("Diálogos por poción")]
    public CustomerPotionDialogue[] potionDialogues;
}