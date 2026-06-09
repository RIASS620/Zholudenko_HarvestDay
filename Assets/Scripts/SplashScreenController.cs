using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenController : MonoBehaviour
{
    [Header("Налаштування часу")]
    public float delayBeforeFadeIn = 0.5f; 
    public float fadeInDuration = 1.8f;    
    public float displayDuration = 2.5f;  
    public float fadeOutDuration = 1.5f;   

    [Header("Назва сцени Головного Меню")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Посилання на Canvas Group всього Canvas")]
    public CanvasGroup splashCanvasGroup; 

    void Start()
    {
        Time.timeScale = 1f;

        // На початку робимо Canvas невидимим
        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0f;
        }

        StartCoroutine(PlaySplashScreen());
    }

    private IEnumerator PlaySplashScreen()
    {
        yield return new WaitForSeconds(delayBeforeFadeIn);

        // плавна поява іконки та тексту
        float counter = 0f;
        while (counter < fadeInDuration)
        {
            counter += Time.deltaTime;
            float progress = counter / fadeInDuration;

            if (splashCanvasGroup != null)
            {
                // SmoothStep робить старт і кінець появи дуже м'якими
                splashCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
            }
            yield return null;
        }
        if (splashCanvasGroup != null) splashCanvasGroup.alpha = 1f;

        // студія абоба
        yield return new WaitForSeconds(displayDuration);

        //плавне згасання
        counter = 0f;
        while (counter < fadeOutDuration)
        {
            counter += Time.deltaTime;
            float progress = counter / fadeOutDuration;

            if (splashCanvasGroup != null)
            {
                splashCanvasGroup.alpha = Mathf.SmoothStep(1f, 0f, progress);
            }
            yield return null;
        }
        if (splashCanvasGroup != null) splashCanvasGroup.alpha = 0f;

        // Невеличка пауза перед меню
        yield return new WaitForSeconds(0.3f);

        // головне меню
        SceneManager.LoadScene(mainMenuSceneName);
    }
}