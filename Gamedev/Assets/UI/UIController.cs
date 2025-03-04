using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using TMPro;


public class UIController : MonoBehaviour
{
    public VisualElement ui;
    public Button Shop;
    public Button Inventory;
    public Button PlayPause;
    public Button Settings;
    public Button NewDay;
    public GameObject shopPanel; // Reference to the Shop Canvas
    public GameObject inventoryPanel; // Reference to the Inventory Canvas
    private int coinCount = 20; // Default coin count
    public TMP_Text coinUI;

    //for build system
    public PlacementSystem placementSystem;

    //for playpause button
    private bool isGamePaused = false;

    //for settings button
    public VisualElement settingsPanel;

    //for end of the day pop up
    public VisualElement endDayScreen;

    //for ui phases
    public VisualElement dayUI;
    public VisualElement nightUI;

    //for build ui
    public VisualElement buildUI;

    //for Progress bar for Phases---------------------------------------------------------------------------------
    public ProgressBar phaseTimer;
    private float timerDuration = 100f; //5min
    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    //for night phase
    public bool isNightPhase = false;

    //brightness slider
    public Slider brightnessSlider;

    //volume slider
    public Slider volumeSlider;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        placementSystem = FindObjectOfType<PlacementSystem>();


        // Initialize UI
        UpdateCoinUI();

        // Hide the settings panel initially
        settingsPanel = ui.Q<VisualElement>("settingsPanel");
        if (settingsPanel != null)
        {
            settingsPanel.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("Settings Panel not found");
        }

        //Hide the night phase screen till the day phase is over
        endDayScreen = ui.Q<VisualElement>("endDayScreen");
        if (endDayScreen != null)
        {
            endDayScreen.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("End Day Screen not found");
        }

        // Day user interface
        dayUI = ui.Q<VisualElement>("dayUI");
        if (dayUI != null)
        {
            dayUI.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogError("End Day Screen not found");
        }

        // Night user interface
        nightUI = ui.Q<VisualElement>("nightUI");
        if (nightUI != null)
        {
            nightUI.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("Night UI not found");
        }

        // Build user interface
        buildUI = ui.Q<VisualElement>("buildUI");
        if (buildUI != null)
        {
            buildUI.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("Build UI not found.");
        }


        //Progress Bar
        phaseTimer = ui.Q<ProgressBar>("phaseTimer");
        if (phaseTimer != null)
        {
            Debug.Log("TimerProgressBar found.");
            phaseTimer.lowValue = 0f;
            phaseTimer.highValue = timerDuration;
            phaseTimer.value = 0f; // Start the timer at 0
        }
        else
        {
            Debug.LogError("TimerProgressBar not found in the UI Document. Check the name and hierarchy.");
        }

        //Brightness Slider
        brightnessSlider = ui.Q<Slider>("BrightnessSlider");
        if (brightnessSlider != null)
        {
            Debug.Log("BrightnessSlider found.");
            brightnessSlider.RegisterValueChangedCallback(OnBrightnessChanged);
        }
        else
        {
            Debug.LogError("BrightnessSlider not found in the UI Document. Check the name and hierarchy.");
        }

        //Volume Slider
        /*
        volumeSlider = ui.Q<Slider>("VolumeSlider");

        if (volumeSlider != null)
        {
            Debug.Log("VolumeSlider found.");
            volumeSlider.RegisterValueChangedCallback(OnVolumeChanged); // Add event listener
        }
        else
        {
            Debug.LogError("VolumeSlider not found in the UI Document. Check the name and hierarchy.");
        }
        */

        if (shopPanel == null)
        {
            Debug.LogError("Shop Panel is NOT assigned in the Inspector! Please assign it.");
        }
        else
        {
            Debug.Log("Shop Panel assigned successfully.");
            shopPanel.SetActive(false); // Start hidden
        }

        if (inventoryPanel == null)
        {
            Debug.LogError("inventory Panel is NOT assigned in the Inspector! Please assign it.");
        }
        else
        {
            Debug.Log("Inventory Panel assigned successfully.");
            inventoryPanel.SetActive(false); // Start hidden
        }

    }

    private void OnEnable()
    {
        //SideBar Buttons

        Shop = ui.Q<Button>("Shop");
        if (Shop != null)
        {
            Shop.clicked += OnShopButtonClicked;
        }

        Inventory = ui.Q<Button>("Inventory");
        if (Inventory != null)
        {
            Inventory.clicked += OnInventoryButtonClicked;
        }

        NewDay = ui.Q<Button>("NewDay");
        if (NewDay != null)
        {
            NewDay.clicked += OnNewDayButtonClicked;
        }

        PlayPause = ui.Q<Button>("PlayPause");
        if (PlayPause != null)
        {
            PlayPause.clicked += OnPlayPauseButtonClicked;
        }

        Settings = ui.Q<Button>("Settings");
        if (Settings != null)
        {
            Settings.clicked += OnSettingsButtonClicked;

        }
    }

    private void Update()
    {
        // running and the game is not paused
        if (isTimerRunning && !isGamePaused && !isNightPhase)
        {
            elapsedTime += Time.deltaTime;

            // ProgressBar value changing
            if (phaseTimer != null)
            {
                phaseTimer.value = elapsedTime;


                if (elapsedTime >= timerDuration)
                {
                    isTimerRunning = false;
                    Debug.Log("Timer completed!");

                    //trigger event for next phase
                    StartCoroutine(SwitchToNightPhase());
                }
            }
        }
    }

    //BRIGHTNESS SLIDER--------------------------------------------------------------------------------------------
    private void OnBrightnessChanged(ChangeEvent<float> evt)
    {
        float brightnessValue = evt.newValue;
        Debug.Log($"Brightness: {brightnessValue}");

        // Adjust ambient light (alternative approach)
        RenderSettings.ambientLight = new Color(brightnessValue, brightnessValue, brightnessValue);
    }

    //Volume SLIDER--------------------------------------------------------------------------------------------
    /*
    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        float volumeValue = evt.newValue;
        Debug.Log($"Volume changed to: {volumeValue}");

        // Update the volume in the Audio Mixer
        if (audioMixer != null)
        {
            // Convert the slider value to decibels (dB)
            float volumeInDecibels = Mathf.Log10(volumeValue) * 20;
            if (volumeValue <= 0.0001) 
            {
                volumeInDecibels = -80f; 
            }

            audioMixer.SetFloat("MasterVolume", volumeInDecibels);
        }
        else
        {
            Debug.LogError("AudioMixer is not assigned.");
        }
    }
    */

    private void OnShopButtonClicked()
    {

        shopPanel.SetActive(true);

    }

    private void OnInventoryButtonClicked()
    {

        inventoryPanel.SetActive(true);

    }

    //NEXT DAY PHASE 
    private void OnNewDayButtonClicked()
    {
        isNightPhase = false;
        elapsedTime = 0f;
        isTimerRunning = true;

        if (phaseTimer != null)
        {
            phaseTimer.value = 0f;
        }

        // Change UI
        if (endDayScreen != null)
        {
            endDayScreen.style.display = DisplayStyle.None;
        }
        if (dayUI != null)
        {
            dayUI.style.display = DisplayStyle.Flex;
        }
        if (nightUI != null)
        {
            nightUI.style.display = DisplayStyle.None;
        }
    }


    //NIGHT PHASE
    private IEnumerator SwitchToNightPhase()
    {
        // Switch to night phase
        isNightPhase = true;

        // Makes end of day screen appear
        if (dayUI != null)
        {
            dayUI.style.display = DisplayStyle.None;
        }

        if (endDayScreen != null)
        {
            endDayScreen.style.display = DisplayStyle.Flex;

            // User can click away screen pop up
            endDayScreen.RegisterCallback<ClickEvent>(evt =>
            {
                endDayScreen.style.display = DisplayStyle.None;

                //displays night ui
                if (nightUI != null)
                {
                    nightUI.style.display = DisplayStyle.Flex;
                }
            });
        }

        if (Shop != null)
        {
            Shop.style.display = DisplayStyle.Flex;
        }

        // Stays night until new day button is clicked
        yield return null;
    }

    //PAUSE AND PLAY GAME
    private void OnPlayPauseButtonClicked()
    {
        Debug.Log("PlayPause Button Clicked");

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0;
            Debug.Log("Game Paused");
            PlayPause.text = "Play";
            SetButtonInteractivity(false);
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Game Resumed");
            PlayPause.text = "Pause";
            SetButtonInteractivity(true);
        }

    }

    private void SetButtonInteractivity(bool isEnabled)
    {
        Shop.SetEnabled(isEnabled);
        Settings.SetEnabled(isEnabled);
        Settings.SetEnabled(isEnabled);
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Button Clicked");

        if (settingsPanel != null)
        {
            if (settingsPanel.style.display == DisplayStyle.None)
            {
                settingsPanel.style.display = DisplayStyle.Flex;
            }
            else
            {
                settingsPanel.style.display = DisplayStyle.None;
            }
        }


    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    public void SpendCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            UpdateCoinUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    private void UpdateCoinUI()
    {
        if (coinUI != null)
        {
            coinUI.text = "Coins: " + coinCount.ToString();
        }
        else
        {
            Debug.LogError("coinText is not assigned in UIController!");
        }
    }

    public int GetCoins()
    {
        return coinCount;
    }

    public bool IsNightPhase
    {
        get { return isNightPhase; }
    }

}