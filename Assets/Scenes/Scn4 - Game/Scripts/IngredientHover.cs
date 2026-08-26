using UnityEngine;
using UnityEngine.InputSystem;

public class IngredientHover : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject ghost;
    [SerializeField] private Collider2D cauldronCollider;
    [SerializeField] private PotionMixer potionMixer;

    private bool isDragging;
    private IngredientStats ingredientStats;
    private Vector3 ghostInitialLocalPosition;
    private Quaternion ghostInitialLocalRotation;

    private void Awake()
    {
        ingredientStats = GetComponent<IngredientStats>();
        ghostInitialLocalPosition = ghost.transform.localPosition;
        ghostInitialLocalRotation = ghost.transform.localRotation;
        ghost.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (!isDragging)
            highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        isDragging = true;
        ghost.SetActive(true);
    }

    private void Update()
    {
        if (!isDragging) return;

        Mouse mouse = Mouse.current;
        Camera mainCamera = Camera.main;

        if (mouse == null || mainCamera == null)
            return;

        Vector3 mousePosition = mouse.position.ReadValue();
        mousePosition.z = Mathf.Abs(ghost.transform.position.z - mainCamera.transform.position.z);
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = ghost.transform.position.z;
        ghost.transform.position = mousePosition;

        if (mouse.leftButton.wasReleasedThisFrame)
            StopDragging();
    }

    private void StopDragging()
    {
        bool droppedOnCauldron = cauldronCollider != null &&
                                cauldronCollider.OverlapPoint(ghost.transform.position);

        if (droppedOnCauldron && potionMixer != null && ingredientStats != null)
            potionMixer.AddIngredient(ingredientStats);

        isDragging = false;
        highlight.SetActive(false);
        ghost.transform.localPosition = ghostInitialLocalPosition;
        ghost.transform.localRotation = ghostInitialLocalRotation;
        ghost.SetActive(false);
    }
}
