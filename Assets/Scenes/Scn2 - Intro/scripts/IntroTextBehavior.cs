using UnityEngine;

using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private GameObject nextPromptIndicator; // Arrow/prompt object

    [Header("Settings")]
    [SerializeField] private float timePerCharacter = 0.05f;
    [SerializeField] private string nextSceneName; // Name of the scene to load

    private string fullText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool textCompleted = false;

    private void Start()
    {
        if (nextPromptIndicator != null) nextPromptIndicator.SetActive(false);
        TriggerText(textComponent.text);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("click");
            HandlePlayerClick();
        }
    }
    public void TriggerText(string textToType)
    {
        fullText = textToType;
        textCompleted = false;
        if (nextPromptIndicator != null) nextPromptIndicator.SetActive(false);

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(timePerCharacter);
        }

        CompleteTextFlow();
    }

    private void HandlePlayerClick()
    {
        // Feature 2: Early Skip Button logic
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            textComponent.maxVisibleCharacters = fullText.Length; // Show all text instantly
            CompleteTextFlow();
        }
        // Feature 1: Jump to next scene logic
        else if (textCompleted)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("Next scene name is empty! Assign it in the Inspector.");
            }
        }
    }

    private void CompleteTextFlow()
    {
        isTyping = false;
        textCompleted = true;

        // Show the visual indicator (e.g., flashing arrow or "Click to Continue" text)
        if (nextPromptIndicator != null)
        {
            nextPromptIndicator.SetActive(true);
        }
    }
}