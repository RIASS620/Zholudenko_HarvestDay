using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Аудіо для Меню")]
    public AudioSource sfxSource;
    public AudioClip buttonClickSound;  // Звук кліку кнопки

    public void StartGame()
    {
        PlayClickAndExecute("SampleScene");
    }

    public void ExitGame()
    {
        Debug.Log("Гра закрилася!");
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
            //корутина затримки
            StartCoroutine(WaitAndQuit());
        }
        else
        {
            Application.Quit();
        }
    }

    private void PlayClickAndExecute(string sceneName)
    {
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
            StartCoroutine(WaitAndLoad(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator WaitAndLoad(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.15f); //пауза для кліку
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator WaitAndQuit()
    {
        yield return new WaitForSecondsRealtime(0.15f); //пауза для кліку перед закриттям
        Application.Quit();
    }
}