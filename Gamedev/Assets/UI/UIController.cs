using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UI;



public class UIController : MonoBehaviour
{
    public VisualElement ui;
    public static UIController Instance { get; private set; }


    public UnityEngine.UIElements.Button Harvest;
    public UnityEngine.UIElements.Button Spray;
    public UnityEngine.UIElements.Button Move;
    public UnityEngine.UIElements.Button Shop;
    public UnityEngine.UIElements.Button Inventory;
    public UnityEngine.UIElements.Button PlayPause;
    public UnityEngine.UIElements.Button Settings;
    public UnityEngine.UIElements.Button Build;
    public UnityEngine.UIElements.Button NewDay;
    public GameObject shopPanel; // Reference to the Shop Canvas
    public GameObject inventoryPanel; // Reference to the Inventory Canvas
    private int coinCount = 20; // Default coin count
    public TMP_Text coinUI;

    // some inventory buttons
    public UnityEngine.UIElements.Button Broccoli;
    public UnityEngine.UIElements.Button Carrot;
    public UnityEngine.UIElements.Button Cauliflower;
    public UnityEngine.UIElements.Button Lettuce;
    public UnityEngine.UIElements.Button Pumpkin;
    public UnityEngine.UIElements.Button Watermelon;

    //for build system
    public PlacementSystem placementSystem;
    public static bool isBuild = false;

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
    private float timerDuration = 10f; //5min
    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    //for night phase
    private bool isNightPhase = false;

    //brightness slider
    public UnityEngine.UIElements.Slider brightnessSlider;

    //volume slider
    public UnityEngine.UIElements.Slider volumeSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; 
        DontDestroyOnLoad(gameObject); 
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
        brightnessSlider = ui.Q<UnityEngine.UIElements.Slider>("BrightnessSlider");
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

        //bottomContainer Buttons
        Harvest = ui.Q<UnityEngine.UIElements.Button> ("Harvest");
        if (Harvest != null)
        {
            Harvest.clicked += OnHarvestButtonClicked;
        }

        Spray = ui.Q<UnityEngine.UIElements.Button> ("Spray");
        if (Spray != null)
        {
            Spray.clicked += OnSprayButtonClicked;
        }

        Move = ui.Q<UnityEngine.UIElements.Button> ("Move");
        if (Move != null)
        {
            Move.clicked += OnMoveButtonClicked;
        }

        //SideBar Buttons

        Shop = ui.Q<UnityEngine.UIElements.Button> ("Shop");
        if (Shop != null)
        {
            Shop.clicked += OnShopButtonClicked;
        }

        Inventory = ui.Q<UnityEngine.UIElements.Button> ("Inventory");
        if (Inventory != null)
        {
            Inventory.clicked += OnInventoryButtonClicked;
        }


        Build = ui.Q<UnityEngine.UIElements.Button> ("Build");
        if (Shop != null)
        {
            Build.clicked += OnBuildButtonClicked;
        }

        NewDay = ui.Q< UnityEngine.UIElements.Button> ("NewDay");
        if (NewDay != null)
        {
            NewDay.clicked += OnNewDayButtonClicked;
        }

        PlayPause = ui.Q<UnityEngine.UIElements.Button> ("PlayPause");
        if (PlayPause != null)
        {
            PlayPause.clicked += OnPlayPauseButtonClicked;
        }

        Settings = ui.Q<UnityEngine.UIElements.Button> ("Settings");
        if (Settings != null)
        {
            Settings.clicked += OnSettingsButtonClicked;

        }

        //inventory buttons
        Broccoli = ui.Q<UnityEngine.UIElements.Button> ("Broccoli");
        if (Broccoli != null)
            Broccoli.clicked += () => OnVegetableButtonClicked(0);

        Carrot = ui.Q<UnityEngine.UIElements.Button> ("Carrot");
        if (Carrot != null)
            Carrot.clicked += () => OnVegetableButtonClicked(1);

        Cauliflower = ui.Q<UnityEngine.UIElements.Button> ("Cauliflower");
        if (Cauliflower != null)
            Cauliflower.clicked += () => OnVegetableButtonClicked(2);

        Lettuce = ui.Q<UnityEngine.UIElements.Button> ("Lettuce");
        if (Lettuce != null)
            Lettuce.clicked += () => OnVegetableButtonClicked(3);

        Pumpkin = ui.Q<UnityEngine.UIElements.Button> ("Pumpkin");
        if (Pumpkin != null)
            Pumpkin.clicked += () => OnVegetableButtonClicked(4);

        Watermelon = ui.Q<UnityEngine.UIElements.Button> ("Watermelon");
        if (Watermelon != null)
            Watermelon.clicked += () => OnVegetableButtonClicked(5);
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

    private void OnHarvestButtonClicked()
    {
        Debug.Log("Harvest Button Clicked");
    }

    private void OnSprayButtonClicked()
    {
        Debug.Log("Spray Button Clicked");
    }

    private void OnMoveButtonClicked()
    {
        Debug.Log("Move Button Clicked");
    }

    private void OnShopButtonClicked()
    {

        shopPanel.SetActive(true);

    }

    private void OnInventoryButtonClicked()
    {

        inventoryPanel.SetActive(true);

    }

    private void OnBuildButtonClicked()
    {
        Debug.Log("Build Button Clicked");

        if (buildUI != null)
        {
            bool isActive = buildUI.style.display == DisplayStyle.Flex;
            buildUI.style.display = isActive ? DisplayStyle.None : DisplayStyle.Flex;
        }

        if (isBuild)
        {
            Debug.Log("Stopping Placement System");
            placementSystem.StopPlacementWrapper();
            isBuild = false; // Disable build mode

        }
        else
        {
            Debug.Log("Starting Placement System");
            isBuild = true; // Enable build mode

        }
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

        if (Build != null)
        {
            Build.style.display = DisplayStyle.Flex;
        }

        if (Shop != null)
        {
            Shop.style.display = DisplayStyle.Flex;
        }

        // Stays night until new day button is clicked
        yield return null;
    }

    private void OnVegetableButtonClicked(int vegetableIndex)
    {
        Debug.Log($"Inventory Button Clicked - Starting Placement System for ID: {vegetableIndex}");

        if (placementSystem != null)
        { 

            // Start placement
            placementSystem.StartPlacement(vegetableIndex);
        }
        else
        {
            Debug.LogError("PlacementSystem not found!");
        }
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
        Harvest.SetEnabled(isEnabled);
        Spray.SetEnabled(isEnabled);
        Move.SetEnabled(isEnabled);
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
            else { 
            
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

}