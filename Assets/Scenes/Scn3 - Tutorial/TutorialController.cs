using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public GameObject[] tutorialCanvases;
    public string nextSceneName;
    
    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentCanvas();
    }

    void Update()
    {
        if ((Mouse.current.leftButton.wasPressedThisFrame))
        {
            AdvanceTutorial();
        }
    }

    void AdvanceTutorial()
    {
        currentIndex++;

        if (currentIndex < tutorialCanvases.Length)
        {
            ShowCurrentCanvas();
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowCurrentCanvas()
    {
        for (int i = 0; i < tutorialCanvases.Length; i++)
        {
            if (tutorialCanvases[i] != null)
            {
                tutorialCanvases[i].SetActive(i == currentIndex);
            }
        }
    }
}