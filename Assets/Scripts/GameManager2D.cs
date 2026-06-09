using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D Instance;

    [Header("Параметри гри")]
    public float roundDuration = 30f;
    public float timeRemaining = 30f;
    public int score = 0;
    public int targetScore = 5;
    public int currentRound = 1;
    public float spawnInterval = 1.2f;

    [Header("Шанси спавну")]
    [Range(0, 100)] public int goldenCarrotChance = 10;

    [Header("Посилання на ui тексти")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI goldenCarrotText;
    public TextMeshProUGUI finalResultText;

    [Header("Панелі інтерфейсу")]
    public GameObject birdObject;
    public GameObject gameOverPanel;
    public GameObject upgradePanel;
    public GameObject pausePanel;

    [Header("Префаби")]
    public GameObject carrotPrefab;
    public GameObject goldenCarrotPrefab;

    [Header("Кнопки Апгрейдів")]
    public Button[] upgradeButtons;

    [Header("Текстури Апгрейдів")]
    public Sprite[] upgradeSprites;

    [Header("Звуки, музика")]
    public AudioSource musicSource;       
    public AudioSource sfxSource;        
    public AudioClip carrotSound;
    public AudioClip goldenCarrotSound;
    public AudioClip poopSound;
    public AudioClip gameOverSound;
    public AudioClip buttonClickSound;   

    private bool isGameActive = true;
    private bool isPaused = false;
    private Coroutine spawnCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        StartNewRoundLogic();

        // повернення гучності на старті і перезапуску
        if (musicSource != null)
        {
            musicSource.volume = 1f;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                if (upgradePanel.activeSelf || gameOverPanel.activeSelf) return;
                PauseGame();
            }
        }

        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
        }

        if (timeRemaining <= 0) LoseGame();
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        UpdateUI();

        if (score >= targetScore) ShowUpgrades();
    }

    public void HitPoop()
    {
        if (!isGameActive) return;
        timeRemaining -= timeRemaining * 0.20f;
        if (timeRemaining < 0) timeRemaining = 0;
    }

    void UpdateUI()
    {
        if (roundText != null)
        {
            roundText.text = "Day: " + currentRound.ToString();
        }

        if (scoreText != null)
        {
            scoreText.text = "Goal: " + $"{score} / {targetScore}";
        }
        if (goldenCarrotText != null)
        {
            goldenCarrotText.text = "Golden Carrot chance: " + $"{goldenCarrotChance}%";
        }
    }

    IEnumerator SpawnCrystalsOverTime(int totalCount)
    {
        for (int i = 0; i < totalCount; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!isGameActive) yield break;

            float randomX = Random.Range(-8f, 8f);
            float randomY = Random.Range(-4.2f, 1.4f);

            Vector3 randomPos = new Vector3(randomX, randomY, 0f);

            GameObject prefabToSpawn = carrotPrefab;
            if (Random.Range(0, 100) < goldenCarrotChance)
            {
                prefabToSpawn = goldenCarrotPrefab;
            }

            Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
        }
    }

    void ShowUpgrades()
    {
        isGameActive = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);

        SetupRandomUpgrades();
        upgradePanel.SetActive(true);
        SetMainGameplayUI(false);

        if (goldenCarrotText != null) goldenCarrotText.gameObject.SetActive(true);
    }

    void SetupRandomUpgrades()
    {
        List<int> pool = new List<int> { 0, 1, 2, 3, 4 };
        List<int> chosenUpgrades = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            chosenUpgrades.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        for (int i = 0; i < 3; i++)
        {
            int upgradeID = chosenUpgrades[i];
            Button btn = upgradeButtons[i];

            btn.onClick.RemoveAllListeners();

            //звук кнопки апгрейду
            btn.onClick.AddListener(PlayButtonClickSound);

            if (upgradeSprites != null && upgradeID < upgradeSprites.Length)
            {
                btn.image.sprite = upgradeSprites[upgradeID];
            }

            if (upgradeID == 0) btn.onClick.AddListener(ApplySpeedUpgrade);
            else if (upgradeID == 1) btn.onClick.AddListener(ApplyTimeUpgrade);
            else if (upgradeID == 2) btn.onClick.AddListener(ApplyTargetUpgrade);
            else if (upgradeID == 3) btn.onClick.AddListener(ApplyGoldenChanceUpgrade);
            else if (upgradeID == 4) btn.onClick.AddListener(ApplySpawnRateUpgrade);
        }
    }

    void ApplySpeedUpgrade() { FindAnyObjectByType<PlayerController2D>().speed += 2f; NextRound(); }
    void ApplyTimeUpgrade() { roundDuration += 10f; NextRound(); }
    void ApplyTargetUpgrade() { targetScore -= 2; NextRound(); }
    void ApplyGoldenChanceUpgrade() { goldenCarrotChance += 1; if (goldenCarrotChance > 100) goldenCarrotChance = 100; NextRound(); }
    void ApplySpawnRateUpgrade() { spawnInterval = Mathf.Max(0.2f, spawnInterval - 0.15f); NextRound(); }

    void NextRound()
    {
        currentRound++;
        targetScore += 6;
        if (targetScore < 1) targetScore = 1;

        timeRemaining = roundDuration;
        score = 0;

        upgradePanel.SetActive(false);

        ClearTag("Respawn");
        ClearTag("Finish");
        ClearTag("Poop");

        StartNewRoundLogic();
    }

    void StartNewRoundLogic()
    {
        if (birdObject != null)
        {
            birdObject.SetActive(true);
            SpriteRenderer birdRender = birdObject.GetComponent<SpriteRenderer>();
            if (birdRender != null) birdRender.enabled = true;
        }

        int crystalsToSpawn = targetScore + 5;
        spawnInterval = Mathf.Max(0.3f, spawnInterval - 0.03f);

        isGameActive = true;
        spawnCoroutine = StartCoroutine(SpawnCrystalsOverTime(crystalsToSpawn));

        SetMainGameplayUI(true);

        if (goldenCarrotText != null) goldenCarrotText.gameObject.SetActive(false);

        UpdateUI();
    }

    void ClearTag(string tagName)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject obj in objects) Destroy(obj);
    }

    public void LoseGame()
    {
        isGameActive = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if (birdObject != null) birdObject.SetActive(false);

        SetMainGameplayUI(false);
        if (goldenCarrotText != null) goldenCarrotText.gameObject.SetActive(false);

        if (finalResultText != null)
        {
            finalResultText.text = "Days Survived: " + currentRound.ToString();
        }

        //звук програшу
        if (musicSource != null) musicSource.Stop();
        PlaySound(gameOverSound);

        gameOverPanel.SetActive(true);
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            //звук при Time.timeScale = 0
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position, sfxSource != null ? sfxSource.volume : 1f);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //затримка для рестарту, щоб звук встиг програтися перед прибиранням сцени
    public void RestartGame()
    {
        PlayButtonClickSound();
        StartCoroutine(WaitAndLoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    //заглушка музики
    public void PauseGame()
    {
        isPaused = true;
        isGameActive = false;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        if (musicSource != null) musicSource.volume = 0.3f;

        if (goldenCarrotText != null) goldenCarrotText.gameObject.SetActive(true);
    }

    // Повернення гучності після паукзи
    public void ResumeGame()
    {
        PlayButtonClickSound();
        isPaused = false;
        isGameActive = true;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (musicSource != null) musicSource.volume = 1f;

        if (goldenCarrotText != null) goldenCarrotText.gameObject.SetActive(false);
    }

    public void RestartFromPause()
    {
        Time.timeScale = 1f;
        RestartGame();
    }

    //затримка для звук перед поверненням в меню
    public void GoToMainMenu()
    {
        PlayButtonClickSound();
        Time.timeScale = 1f;
        StartCoroutine(WaitAndLoadSceneByName("MainMenu"));
    }

    private IEnumerator WaitAndLoadScene(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator WaitAndLoadSceneByName(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(sceneName);
    }

    private void SetMainGameplayUI(bool state)
    {
        if (roundText != null) roundText.gameObject.SetActive(state);
        if (scoreText != null) scoreText.gameObject.SetActive(state);
        if (timerText != null) timerText.gameObject.SetActive(state);
    }
}