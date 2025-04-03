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
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public VisualElement ui;
    public static UIController Instance { get; private set; } // Singleton pattern

    private Label coinsLabel;     // Reference to the UXML coins label
    private Label coinsLabelNight;
    private int coinCount = 20;    // Default coin amount
    private int dailyGoal = 15;// Goal for the day (can be dynamic)


    public UnityEngine.UIElements.Button Harvest;
    public UnityEngine.UIElements.Button Spray;
    public UnityEngine.UIElements.Button Move;
    public UnityEngine.UIElements.Button Shop;
    public UnityEngine.UIElements.Button Inventory;
    public UnityEngine.UIElements.Button PlayPause;
    public UnityEngine.UIElements.Button Settings;
    public UnityEngine.UIElements.Button NewDay;
    public GameObject shopPanel; // Reference to the Shop Canvas
    public GameObject inventoryPanel; // Reference to the Inventory Canvas
    public TMP_Text coinsLabelShop; // Shop panel coin label

    //EOD
    private Label goalAmountLabel;
    private Label revenueEarnedLabel;
    private Label resultTextLabel;
    private Label summaryMessageLabel;
    private Label currentMoneyLabel;
    private Label moneyGoalLabel;
    private Label warningsLabel;
    private UnityEngine.UIElements.Button continueButton;
    private UnityEngine.UIElements.Button objectiveButton;
    private UnityEngine.UIElements.Button StartButton;
    private UnityEngine.UIElements.Button SettingsButton;

    // sound effects
    public AudioSource BG_audioSFX;
    public AudioClip BGTitleAndNightSFX;
    public AudioClip BGDaySFX;

    //sound effect adjustments
    private float BGVolume = 0.008f; // lower bg music
    private float BG_Pitch = 1f; // slow/fast day time music

    //for playpause button
    private bool isGamePaused = false;

    //for settings button
    public VisualElement settingsPanel;

    //for end of the day pop up
    public VisualElement endDayScreen;

    //for ui phases
    public VisualElement dayUI;
    public VisualElement nightUI;

    public VisualElement objectivesScreen;

    public VisualElement GameBackground;


    //for Progress bar for Phases---------------------------------------------------------------------------------
    public ProgressBar phaseTimer;
    private float timerDuration = 100f; //5min
    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    //for night phase
    public bool isNightPhase = true;

    //brightness slider
    public UnityEngine.UIElements.Slider brightnessSlider;

    //volume slider
    public UnityEngine.UIElements.Slider volumeSlider;

    //plots
    public CarrotGrowth carrotGrowth;
    public BroccoliGrowth broccoliGrowth;
    public CauliflowerGrowth cauliflowerGrowth;
    public LettuceGrowth lettuceGrowth;
    public PumpkinGrowth pumpkinGrowth;
    public WatermelonGrowth watermelonGrowth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying duplicate UIController");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ui = GetComponent<UIDocument>().rootVisualElement;

        // End of Day UI Elements
        goalAmountLabel = ui.Q<Label>("goalAmount");
        revenueEarnedLabel = ui.Q<Label>("revenueEarned");
        resultTextLabel = ui.Q<Label>("result");
        summaryMessageLabel = ui.Q<Label>("summaryMessage");
        continueButton = ui.Q<UnityEngine.UIElements.Button>("continueButton");


        //objectives element 
        currentMoneyLabel = ui.Q<Label>("currentMoney");
        moneyGoalLabel = ui.Q<Label>("moneyGoal");
        warningsLabel = ui.Q<Label>("warningMessage");

        // Get all Scenes
        objectivesScreen = ui.Q<VisualElement>("objectivesScreen");
        GameBackground = ui.Q<VisualElement>("GameBackground");
        dayUI = ui.Q<VisualElement>("dayUI");
        endDayScreen = ui.Q<VisualElement>("endDayScreen");
        nightUI = ui.Q<VisualElement>("nightUI");

        objectiveButton = ui.Q<UnityEngine.UIElements.Button>("objectiveButton");
        StartButton = ui.Q<UnityEngine.UIElements.Button>("StartButton");
        SettingsButton = ui.Q<UnityEngine.UIElements.Button>("SettingsButton");

        // Hide all screens EXCEPT nightUI at the start
        objectivesScreen.style.display = DisplayStyle.None;
        dayUI.style.display = DisplayStyle.None;
        endDayScreen.style.display = DisplayStyle.None;
        nightUI.style.display = DisplayStyle.None;
        GameBackground.style.display = DisplayStyle.Flex;

        // Add button click event to transition from Main Menu -> Night UI
        StartButton.clicked += () =>
        {
            RestartGame();
            GameBackground.style.display = DisplayStyle.None;  // Hide Main Menu
            nightUI.style.display = DisplayStyle.Flex;// Start the night
            isNightPhase = true;
        };

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

    private void Start()
    {
        // Play title sound effects
        BG_audioSFX.clip = BGTitleAndNightSFX;
        BG_audioSFX.volume = BGVolume;  // Lower volume
        BG_audioSFX.loop = true; // Enable looping
        BG_audioSFX.Play();

        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get coin labels correctly
        coinsLabel = root.Q<Label>("coinsLabel");  // For dayUI
        coinsLabelNight = root.Q<Label>("coinsLabelNight"); // For nightUI

        if (shopPanel != null)
        {
            coinsLabelShop = shopPanel.transform.Find("CoinUI").GetComponent<TMP_Text>();
        }

        // Debug if missing
        if (coinsLabel == null) Debug.LogError("coinsLabel (Day UI) not found in UXML!");
        if (coinsLabelNight == null) Debug.LogError("coinsLabelNight (Night UI) not found in UXML!");
        if (coinsLabelShop == null) Debug.LogError("coinsLabelShop (Shop UI) not found in GameObject!");

        carrotGrowth = FindObjectOfType<CarrotGrowth>();
        broccoliGrowth = FindObjectOfType<BroccoliGrowth>();
        cauliflowerGrowth = FindObjectOfType<CauliflowerGrowth>();
        lettuceGrowth = FindObjectOfType<LettuceGrowth>();
        pumpkinGrowth = FindObjectOfType<PumpkinGrowth>();
        watermelonGrowth = FindObjectOfType<WatermelonGrowth>();

    UpdateCoinUI();
    }


    private void OnEnable()
    {

        //bottomContainer Buttons
        Harvest = ui.Q<UnityEngine.UIElements.Button>("Harvest");
        if (Harvest != null)
        {
            Harvest.clicked += OnHarvestButtonClicked;
        }

        Spray = ui.Q<UnityEngine.UIElements.Button>("Spray");
        if (Spray != null)
        {
            Spray.clicked += OnSprayButtonClicked;
        }

        Move = ui.Q<UnityEngine.UIElements.Button>("Move");
        if (Move != null)
        {
            Move.clicked += OnMoveButtonClicked;
        }

        //SideBar Buttons

        Shop = ui.Q<UnityEngine.UIElements.Button>("Shop");
        if (Shop != null)
        {
            Shop.clicked += OnShopButtonClicked;
        }

        Inventory = ui.Q<UnityEngine.UIElements.Button>("Inventory");
        if (Inventory != null)
        {
            Inventory.clicked += OnInventoryButtonClicked;
        }


        NewDay = ui.Q<UnityEngine.UIElements.Button>("NewDay");
        if (NewDay != null)
        {
            NewDay.clicked += OnNewDayButtonClicked;
        }

        PlayPause = ui.Q<UnityEngine.UIElements.Button>("PlayPause");
        if (PlayPause != null)
        {
            PlayPause.clicked += OnPlayPauseButtonClicked;
        }

        Settings = ui.Q<UnityEngine.UIElements.Button>("Settings");
        if (Settings != null)
        {
            Settings.clicked += OnSettingsButtonClicked;

        }

    }

    private void Update()
    {
        if (dayUI.style.display != DisplayStyle.Flex)
        {
            return; // Stop execution if Day UI is not active
        }

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



    //NEXT DAY PHASE 
    private void OnNewDayButtonClicked()
    {
        isNightPhase = false;
        elapsedTime = 0f;
        isTimerRunning = true;
        nightUI.style.display = DisplayStyle.None;
        
        // Calculate revenue made during the day
        int currentCoin = coinCount;
        int goalCoin = dailyGoal;

        // Play day bg sound
        BG_audioSFX.clip = BGDaySFX;
        BG_audioSFX.volume = BGVolume;  // Lower volume
        BG_audioSFX.pitch = BG_Pitch; // slow down a little
        BG_audioSFX.loop = true; // Enable looping
        BG_audioSFX.Play();


        // Update Objectives labels
        currentMoneyLabel.text = currentCoin + " Coins";
        moneyGoalLabel.text = goalCoin + " Coins";
        warningsLabel.text = "Need at least " + goalCoin + " Coins for rent by end of\r\nday before the farm goes into foreclosure!";
        objectivesScreen.style.display = DisplayStyle.Flex;

        if (carrotGrowth != null)
        {
            carrotGrowth.SetFarmingMode(true); 
        }
        if (broccoliGrowth != null)
        {
            broccoliGrowth.SetFarmingMode(true);
        }
        if (cauliflowerGrowth != null)
        {
            cauliflowerGrowth.SetFarmingMode(true);
        }
        if (lettuceGrowth != null)
        {
            lettuceGrowth.SetFarmingMode(true);
        }
        if (pumpkinGrowth != null)
        {
            pumpkinGrowth.SetFarmingMode(true);
        }
        if (watermelonGrowth != null)
        {
            watermelonGrowth.SetFarmingMode(true);
        }

        // Remove previous event listeners to prevent stacking
        objectiveButton.clicked -= OnObjectiveButtonClicked;
        objectiveButton.clicked += OnObjectiveButtonClicked;
    }

    private void OnObjectiveButtonClicked()
    {
        objectivesScreen.style.display = DisplayStyle.None;  // Hide Objectives
        dayUI.style.display = DisplayStyle.Flex; // Show the Day UI

        if (phaseTimer != null)
        {
            phaseTimer.value = 0f;
        }

        // Hide night UI and end day screen
        if (endDayScreen != null)
        {
            endDayScreen.style.display = DisplayStyle.None;
        }
        if (nightUI != null)
        {
            nightUI.style.display = DisplayStyle.None;
        }
    }

    //NightPhase
    private IEnumerator SwitchToNightPhase()
    {
        // Switch to night phase
        isNightPhase = true;

        // Play background music
        BG_audioSFX.clip = BGTitleAndNightSFX;
        BG_audioSFX.volume = BGVolume;  // Lower volume
        BG_audioSFX.loop = true; // Enable looping
        BG_audioSFX.Play();

        // Calculate revenue made during the day
        int revenueEarned = coinCount;

        // Goal for the day (can be dynamic)
        bool goalMet = revenueEarned >= dailyGoal;

        // Update End of Day UI labels
        goalAmountLabel.text = dailyGoal + " Coins";
        revenueEarnedLabel.text = revenueEarned + " Coins";

        if (goalMet)
        {
            resultTextLabel.text = "PASSED!";
            resultTextLabel.style.color = new StyleColor(Color.green);
            summaryMessageLabel.text = "Now transitioning to night time, buy more plots to make more earnings!";
            continueButton.text = "Proceed to Night Phase";

        }
        else
        {
            resultTextLabel.text = "FAILED!";
            resultTextLabel.style.color = new StyleColor(Color.red);
            summaryMessageLabel.text = "Sorry, you did not meet your goal... farm went to foreclosure!";
            continueButton.text = "Return to Main Menu";
        }


        // Hide the Day UI and Show End of Day UI
        dayUI.style.display = DisplayStyle.None;
        endDayScreen.style.display = DisplayStyle.Flex;

        // Ensure Build and Shop buttons are available for the night phase
        if (Shop != null) Shop.style.display = DisplayStyle.Flex;

        // Wait for player to click "Continue"
        continueButton.clicked += () =>
        {
            endDayScreen.style.display = DisplayStyle.None;

            if (goalMet)
            {
                //If the goal was met, transition to night UI
                nightUI.style.display = DisplayStyle.Flex;
            }
            else
            {
                // If the player failed, return to the main menu
                GameBackground.style.display = DisplayStyle.Flex;
            }
        };
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
            //SetButtonInteractivity(false);
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Game Resumed");
            PlayPause.text = "Pause";
            //SetButtonInteractivity(true);
        }

    }

    //private void SetButtonInteractivity(bool isEnabled)
    //{
    // Harvest.SetEnabled(isEnabled);
    //Spray.SetEnabled(isEnabled);
    //Move.SetEnabled(isEnabled);
    //Shop.SetEnabled(isEnabled);
    //Settings.SetEnabled(isEnabled);
    //Settings.SetEnabled(isEnabled);
    //}

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
        if (coinsLabel != null)
        {
            coinsLabel.text = "Coins: " + coinCount.ToString();
        }

        if (coinsLabelNight != null)
        {
            coinsLabelNight.text = "Coins: " + coinCount.ToString();
        }
        if (coinsLabelShop != null)
        {
            coinsLabelShop.text = "Coins: " + coinCount.ToString();
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

    public void RestartGame()
    {
        // Hide all UI panels
        if (shopPanel != null) shopPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        // Reset game state variables
        isNightPhase = true;
        elapsedTime = 0f;
        isTimerRunning = false;
        coinCount = 20;
        isGamePaused = false;
        Time.timeScale = 1;

        // Reset all UI
        GameBackground.style.display = DisplayStyle.None;
        dayUI.style.display = DisplayStyle.None;
        endDayScreen.style.display = DisplayStyle.None;
        objectivesScreen.style.display = DisplayStyle.None;
        nightUI.style.display = DisplayStyle.Flex;

        // Hide settings panel if it's open
        if (settingsPanel != null)
        {
            settingsPanel.style.display = DisplayStyle.None;
        }

        // Update coins
        UpdateCoinUI();

        // Reset progress bar
        if (phaseTimer != null)
        {
            phaseTimer.value = 0f;
        }

        Inventory inventory = FindObjectOfType<Inventory>(true);
        if (inventory != null)
        {
            inventory.ResetInventory();
        }
        else
        {
            if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                inventory = inventoryPanel.GetComponent<Inventory>();
                if (inventory != null)
                {
                    inventory.ResetInventory();
                }
            }
        }
        ResetPlayerPosition();
        ResetGameplayObjects();
    }

    private void ResetGameplayObjects()
    {
        GameObject[] carrots = GameObject.FindGameObjectsWithTag("Carrot");
        GameObject[] broccoli = GameObject.FindGameObjectsWithTag("Broccoli");
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag("Mushroom");
        GameObject[] sunflowers = GameObject.FindGameObjectsWithTag("Sunflower");
        GameObject[] corn = GameObject.FindGameObjectsWithTag("Corn");
        GameObject[] cauliflower = GameObject.FindGameObjectsWithTag("Cauliflower");

        // Dont destroy set plot
        foreach (GameObject carrot in carrots)
        {
            if (carrot.name != "carrot-plot-perm")
            {
                Destroy(carrot);
            }
        }

        // Destroy all other plant types
        DestroyTaggedObjects(broccoli);
        DestroyTaggedObjects(mushrooms);
        DestroyTaggedObjects(sunflowers);
        DestroyTaggedObjects(corn);
        DestroyTaggedObjects(cauliflower);
    }

    private void DestroyTaggedObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private void ResetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Player-Spawn");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }
    }
}