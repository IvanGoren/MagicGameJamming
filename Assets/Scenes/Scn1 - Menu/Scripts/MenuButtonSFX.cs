using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonSFX : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    public enum ClickSound
    {
        Confirm,
        Cancel
    }

    [SerializeField] private ClickSound clickSound = ClickSound.Confirm;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuSFXManager.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound == ClickSound.Confirm)
        {
            MenuSFXManager.Instance.PlayConfirm();
        }
        else
        {
            MenuSFXManager.Instance.PlayCancel();
        }
    }
}