using UnityEngine;

using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private TMP_Text nextPromptIndicator; // Arrow/prompt object

    [Header("Settings")]
    [SerializeField] private float timePerCharacter = 0.05f;
    [SerializeField] private string nextSceneName; // Name of the scene to load
    [SerializeField] private float fadeDuration = 2.0f;
    private string fullText;
    private Coroutine typingCoroutine;
    private bool textCompleted = false;

    private void Start()
    {
        TriggerText(textComponent.text);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePlayerClick();
        }
    }
    public void TriggerText(string textToType)
    {
        fullText = textToType;
        textCompleted = false;

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
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
        // Feature 1: Jump to next scene logic
     if (textCompleted)
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
        // Show the visual indicator (e.g., flashing arrow or "Click to Continue" text)
        StartCoroutine(FadeInRoutine());
    }

        private IEnumerator FadeInRoutine()
    {
        float currentTime = 0f;
        Color originalColor = nextPromptIndicator.color;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            
            // Calculate progress between 0 and 1
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            
            // Apply new alpha to text color
            nextPromptIndicator.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return null; // Wait for next frame
        }
        textCompleted = true;
    }
}