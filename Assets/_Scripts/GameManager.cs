using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public LevelCountData levelCountData;
    
    public int count;
    private int countTo3Stars;
    private int countTo2Stars;

    public bool isVictory = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SettingsUI.Instance.musicProcents.text = (SoundManager.Instance.volumeMusic * 100).ToString("0") + "%";
        SettingsUI.Instance.soundsProcents.text = (SoundManager.Instance.volumeSounds * 100).ToString("0") + "%";
        isVictory = false;
        Time.timeScale = 1f;
        SettingsUI.Instance.SliderMusic.onValueChanged.AddListener(HandleMusicVolumeChanged);
        SettingsUI.Instance.SliderSounds.onValueChanged.AddListener(HandleSoundsVolumeChanged);
        
        SettingsUI.Instance.gameObject.SetActive(false);
        PauseUI.Instance.gameObject.SetActive(false);
        LevelCompleteUI.Instance.gameObject.SetActive(false);
        GameOverUI.Instance.gameObject.SetActive(false);
        GameOverUI.Instance.selectGameOverLevels.gameObject.SetActive(false);
        LevelCompleteUI.Instance.selectLevelCompleteLevels.gameObject.SetActive(false);
        GameUI.Instance.pauseGameButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            PauseUI.Instance.gameObject.SetActive(true);
        });
        PauseUI.Instance.continueButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            PauseUI.Instance.gameObject.SetActive(false);
        });
        PauseUI.Instance.retryPauseButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.RestartActiveLevel();
            StarsHandler.Instance.RestartAnimations();
        });
        PauseUI.Instance.settingsButton.onClick.AddListener(() =>
        {
            
            float musicVolume = PlayerPrefs.GetFloat(PlayerPrefsNames.PLAYER_PREFS_NAMES_MUSIC_VOLUME);
            float soundsVolume = PlayerPrefs.GetFloat(PlayerPrefsNames.PLAYER_PREFS_NAMES_SOUNDS_VOLUME);

            SettingsUI.Instance.SliderMusic.value = musicVolume;
            SettingsUI.Instance.SliderSounds.value = soundsVolume;
            SettingsUI.Instance.gameObject.SetActive(true);
        });
        PauseUI.Instance.menuPauseButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.LoadMenu();
        });
        SettingsUI.Instance.closeSettingsButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.gameObject.SetActive(false);
            PlayerPrefs.Save();
        });
        LevelCompleteUI.Instance.nextLvlLevelCompleteButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.LoadNextLevel();
        });
        LevelCompleteUI.Instance.retryLevelCompleteButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.RestartActiveLevel();
        });
        LevelCompleteUI.Instance.selectLevelsLevelCompleteButton.onClick.AddListener(() =>
        {
            LevelCompleteUI.Instance.selectLevelCompleteLevels.gameObject.SetActive(true);
        });
        LevelCompleteUI.Instance.selectLevelsLevelCompleteExitButton.onClick.AddListener(() =>
        {
            LevelCompleteUI.Instance.selectLevelCompleteLevels.gameObject.SetActive(false);
        });
        LevelCompleteUI.Instance.menuLevelCompleteButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.LoadMenu();
        });
        GameOverUI.Instance.retryGameOverButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.RestartActiveLevel();
            
        });
        GameOverUI.Instance.selectLevelsGameOverButton.onClick.AddListener(() =>
        {
            GameOverUI.Instance.selectGameOverLevels.gameObject.SetActive(true);
        });
        GameOverUI.Instance.selectLevelsGameOverExitButton.onClick.AddListener(() =>
        {
            GameOverUI.Instance.selectGameOverLevels.gameObject.SetActive(false);
        });
        GameOverUI.Instance.menuGameOverButton.onClick.AddListener(() =>
        {
            LevelSelector.Instance.LoadMenu();
        });
    }
    
    public void LoadLevelData()
    {
        StarsHandler.Instance.star1Condition = false;
        StarsHandler.Instance.star2Condition = false;
        StarsHandler.Instance.star3Condition = false;
        string currentLevelName = SceneManager.GetActiveScene().name;
        
        LevelCountData.LevelCountsData currentLevelData = levelCountData.levelCountsData.Find(level => level.levelName == currentLevelName);
        if (currentLevelData != null)
        {
            count = currentLevelData.counts;
            countTo3Stars = currentLevelData.countTo3Stars;
            countTo2Stars = currentLevelData.countTo2Stars;
            GameUI.Instance.countOfShots.text = count.ToString();
        }
        else
        {
            Debug.LogError("Level data not found for: " + currentLevelName);
            count = 6;
            countTo3Stars = 3;
            countTo2Stars = 2;
            GameUI.Instance.countOfShots.text = count.ToString();
        }
    }
    
    

    public void Victory()
    {
        if (isVictory)
        {
            Debug.Log("Victory already achieved. Skipping...");
            return;
        }

        Debug.Log("Victory triggered!");
        isVictory = true;
        LevelCompleteUI.Instance.gameObject.SetActive(true);
        StarsEarned();
        Time.timeScale = 0f;
        SoundManager.Instance.PlayWinSound(Camera.main.transform.position, 1f);
    }

    
    public void StarsEarned()
    {
        StarsHandler.Instance.star1Condition = false;
        StarsHandler.Instance.star2Condition = false;
        StarsHandler.Instance.star3Condition = false;
        
        if (count >= countTo3Stars)
        {
            StarsHandler.Instance.star3Condition = true;
        }
        else if (count >= countTo2Stars)
        {
            StarsHandler.Instance.star2Condition = true;
        }
        else
        {
            StarsHandler.Instance.star1Condition = true;
        }
    }

    
    
    public int Count()
    {
        return count;
    }
    
    private void OnDestroy()
    {
        SettingsUI.Instance.SliderMusic.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
        SettingsUI.Instance.SliderSounds.onValueChanged.RemoveListener(HandleSoundsVolumeChanged);
        GameUI.Instance.pauseGameButton.onClick.RemoveAllListeners();
        PauseUI.Instance.continueButton.onClick.RemoveAllListeners();
        PauseUI.Instance.retryPauseButton.onClick.RemoveAllListeners();
        PauseUI.Instance.settingsButton.onClick.RemoveAllListeners();
        PauseUI.Instance.menuPauseButton.onClick.RemoveAllListeners();
        SettingsUI.Instance.closeSettingsButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.nextLvlLevelCompleteButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.retryLevelCompleteButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.selectLevelsLevelCompleteButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.selectLevelsLevelCompleteExitButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.menuLevelCompleteButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.retryGameOverButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.selectLevelsGameOverButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.selectLevelsGameOverExitButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.menuGameOverButton.onClick.RemoveAllListeners();
    }
    
    private void HandleMusicVolumeChanged(float value)
    {
        SoundManager.Instance.musicAudioSource.volume = value;
        PlayerPrefs.SetFloat(PlayerPrefsNames.PLAYER_PREFS_NAMES_MUSIC_VOLUME, value);
        PlayerPrefs.Save();
        SettingsUI.Instance.musicProcents.text = (value * 100).ToString("0") + "%";
    }
    
    private void HandleSoundsVolumeChanged(float value)
    {
        SoundManager.Instance.soundEffectsAudioSource.volume = value;
        PlayerPrefs.SetFloat(PlayerPrefsNames.PLAYER_PREFS_NAMES_SOUNDS_VOLUME, value);
        PlayerPrefs.Save();
        SettingsUI.Instance.soundsProcents.text = (value * 100).ToString("0") + "%";
    }
}