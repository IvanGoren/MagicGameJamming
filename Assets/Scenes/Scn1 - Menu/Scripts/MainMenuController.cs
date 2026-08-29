using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPageCambas;
    public GameObject creditsCambas;

    public void OnStartClick()
    {
        SceneManager.LoadScene("ScnGame");
    }


    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }


    public void OnCreditsClick()
    {
        mainPageCambas.SetActive(false);
        creditsCambas.SetActive(true);
    }

    public void OnCreditsExitClick()
    {
        creditsCambas.SetActive(false);
        mainPageCambas.SetActive(true);
    }
}
