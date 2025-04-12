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

using Debug = UnityEngine.Debug;
using static System.Net.Mime.MediaTypeNames;

public class UIController : MonoBehaviour
{
    public VisualElement ui;
    public static UIController Instance { get; private set; } // Singleton pattern

    private Label coinsLabel;     // Reference to the UXML coins label
    private Label coinsLabelNight;
    private int coinCount = 20;    // Default coin amount
    private int dailyGoal = 15;// Goal for the day (can be dynamic)
    private int dayCount = 0;    // Default coin amount

    public UnityEngine.UIElements.Button Harvest;
    public UnityEngine.UIElements.Button Spray;
    public UnityEngine.UIElements.Button Move;
    public UnityEngine.UIElements.Button Shop;
    public UnityEngine.UIElements.Button Inventory;
    public UnityEngine.UIElements.Button PlayPause;
    public UnityEngine.UIElements.Button Settings;
    public UnityEngine.UIElements.Button NSettings;
    public UnityEngine.UIElements.Button NewDay;
    public GameObject shopPanel; // Reference to the Shop Canvas
    public GameObject inventoryPanel; // Reference to the Inventory Canvas
    public TMP_Text coinsLabelShop; // Shop panel coin label

    //quit button
    public UnityEngine.UIElements.Button QuitButton;
    public UnityEngine.UIElements.Button MQuitButton;

    //EOD
    private Label goalAmountLabel;
    private Label revenueEarnedLabel;
    private Label resultTextLabel;
    private Label summaryMessageLabel;
    private Label currentMoneyLabel;
    private Label moneyGoalLabel;
    private Label warningsLabel;
    private Label dayNumberLabel;
    private Label dayNumberObjectivesLabel;
    private UnityEngine.UIElements.Button continueButton;
    private UnityEngine.UIElements.Button objectiveButton;
    public UnityEngine.UIElements.Button StartButton;
    public UnityEngine.UIElements.Button SettingsButton;

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

    //shop items
    public PlayerAnimator playerAnimator;
    public HelperNPC helperNPC;

    //progression
    public NPCInteraction npcInteraction;
    public DayProgression dayProgression;
    public FlyAI flyAI;

    //cursor
    public Texture2D normalCursor;
    public Texture2D bigCursor;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

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
        dayNumberLabel = ui.Q<Label>("dayNumber");
        dayNumberObjectivesLabel = ui.Q<Label>("dayNumberObjectives");
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

        //main menu quit button
        MQuitButton = ui.Q<UnityEngine.UIElements.Button>("MQuitButton");
        if (MQuitButton != null)
        {
            MQuitButton.clicked += OnQuitButtonClicked;
        }
        else
        {
            Debug.LogError("Quit Button not found in UXML!");
        }

        //quit button
        QuitButton = ui.Q<UnityEngine.UIElements.Button>("QuitButton");
        if (QuitButton != null)
        {
            QuitButton.clicked += OnQuitButtonClicked;
        }
        else
        {
            Debug.LogError("QuitButton not found in UI Document");
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
        playerAnimator = FindObjectOfType<PlayerAnimator>();
        helperNPC = FindObjectOfType<HelperNPC>();
        npcInteraction = FindObjectOfType<NPCInteraction>();
        dayProgression = FindObjectOfType<DayProgression>();
        flyAI = FindObjectOfType<FlyAI>();

        UnityEngine.Cursor.SetCursor(normalCursor, hotspot, cursorMode);
        UnityEngine.Cursor.visible = true;

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
            NewDay.clicked -= OnNewDayButtonClicked;
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

        NSettings = ui.Q<UnityEngine.UIElements.Button>("NSettings");
        if (NSettings != null)
        {
            NSettings.clicked += OnSettingsButtonClicked;

        }

    }

    private void Update()
    {
        if (dayUI.style.display != DisplayStyle.Flex)
        {
            return; 
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

        bool isFarmingActive =
            (carrotGrowth != null && carrotGrowth.CheckFarming()) ||
            (cauliflowerGrowth != null && cauliflowerGrowth.CheckFarming()) ||
            (lettuceGrowth != null && lettuceGrowth.CheckFarming()) ||
            (broccoliGrowth != null && broccoliGrowth.CheckFarming()) ||
            (pumpkinGrowth != null && pumpkinGrowth.CheckFarming()) ||
            (watermelonGrowth != null && watermelonGrowth.CheckFarming());

        if (!isNightPhase && isFarmingActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Carrot") || hit.collider.CompareTag("Cauliflower") || hit.collider.CompareTag("Broccoli") || hit.collider.CompareTag("Corn") || 
                    hit.collider.CompareTag("Mushroom") || hit.collider.CompareTag("NPC1") || hit.collider.CompareTag("NPC2") || hit.collider.CompareTag("NPC3") || hit.collider.CompareTag("NPC4") ||
                    hit.collider.CompareTag("Bowl") || hit.collider.CompareTag("Placemat1") || hit.collider.CompareTag("Placemat2")) 
                {
                    UnityEngine.Cursor.SetCursor(bigCursor, hotspot, cursorMode);
                }
                else
                {
                    UnityEngine.Cursor.SetCursor(normalCursor, hotspot, cursorMode);
                }
            }
            else
            {
                UnityEngine.Cursor.SetCursor(normalCursor, hotspot, cursorMode);
            }
        }
        else
        {
            UnityEngine.Cursor.SetCursor(normalCursor, hotspot, cursorMode);
            UnityEngine.Cursor.visible = true;
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


    //confirm quit and then quit
    private void OnQuitButtonClicked()
    {
    
        var dialogBox = new UnityEngine.UIElements.Box();
        dialogBox.style.position = Position.Absolute;
        dialogBox.style.left = 0;
        dialogBox.style.right = 0;
        dialogBox.style.top = 0;
        dialogBox.style.bottom = 0;
        dialogBox.style.backgroundColor = new Color(0, 0, 0, 0.7f);
        dialogBox.style.justifyContent = Justify.Center;
        dialogBox.style.alignItems = Align.Center;

        // Create dialog content
        var dialogContent = new UnityEngine.UIElements.Box();
        dialogContent.style.backgroundColor = Color.black;
        dialogContent.style.paddingTop = 40;
        dialogContent.style.paddingRight = 40;
        dialogContent.style.paddingBottom = 40;
        dialogContent.style.paddingLeft = 40;

        var questionLabel = new UnityEngine.UIElements.Label("Are you sure you want to quit the farm?");
        questionLabel.style.color = UnityEngine.Color.white;
        questionLabel.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
        questionLabel.style.fontSize = 18;
        questionLabel.style.marginBottom = 20;
        dialogContent.Add(questionLabel);

        // button container
        var buttonContainer = new UnityEngine.UIElements.VisualElement();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.Center;
        buttonContainer.style.marginTop = 20;

        //  confirm button
        var confirmButton = new UnityEngine.UIElements.Button(() => {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        });
        confirmButton.text = "Yes, Quit";
        confirmButton.style.marginRight = 10;
        buttonContainer.Add(confirmButton);

        // Add cancel button
        var cancelButton = new UnityEngine.UIElements.Button(() => {
            ui.Remove(dialogBox);
        });
        cancelButton.text = "Cancel";
        buttonContainer.Add(cancelButton);

        // Add everything to dialog
        dialogContent.Add(buttonContainer);
        dialogBox.Add(dialogContent);
        ui.Add(dialogBox);
    }

    private void OnDisable()
    {
        if (QuitButton != null)
        {
            QuitButton.clicked -= OnQuitButtonClicked;
        }
        if (NewDay != null)
        {
            NewDay.clicked -= OnNewDayButtonClicked;
        }
    }


    //NEXT DAY PHASE 
    private void OnNewDayButtonClicked()
    {
        dayCount++;
        isNightPhase = false;
        elapsedTime = 0f;
        isTimerRunning = true;
        nightUI.style.display = DisplayStyle.None;

        ResetPlayerPosition();

        // Calculate revenue made during the day
        int currentCoin = coinCount;
        int goalCoin = dailyGoal;
        int dayNum = dayCount;
        int earnedAmount = npcInteraction.GetEarned();

        dayProgression.EndDayEarnings(earnedAmount, dayNum);
        goalCoin = dayProgression.NewGoal();

        npcInteraction.NewDayEarned();

        // Play day bg sound
        BG_audioSFX.clip = BGDaySFX;
        BG_audioSFX.volume = BGVolume;  // Lower volume
        BG_audioSFX.pitch = BG_Pitch; // slow down a little
        BG_audioSFX.loop = true; // Enable looping
        BG_audioSFX.Play();

        // Update Objectives labels
        dayNumberLabel.text = "" + dayNum;
        dayNumberObjectivesLabel.text = "Day " + dayNum + " Objectives";
        currentMoneyLabel.text = currentCoin + " Coins";
        moneyGoalLabel.text = goalCoin + " Coins";
        warningsLabel.text = "Need at least " + goalCoin + " Coins for rent by end of\r\nday before the farm goes into foreclosure!";
        objectivesScreen.style.display = DisplayStyle.Flex;

        if(carrotGrowth == null)
        {
            carrotGrowth = FindObjectOfType<CarrotGrowth>();
        }
        if (broccoliGrowth == null)
        {
            broccoliGrowth = FindObjectOfType<BroccoliGrowth>();
        }
        if (cauliflowerGrowth == null)
        {
            cauliflowerGrowth = FindObjectOfType<CauliflowerGrowth>();
        }
        if (lettuceGrowth == null)
        {
            lettuceGrowth = FindObjectOfType<LettuceGrowth>();
        }
        if (pumpkinGrowth == null)
        {
            pumpkinGrowth = FindObjectOfType<PumpkinGrowth>();
        }
        if (watermelonGrowth == null)
        {
            watermelonGrowth = FindObjectOfType<WatermelonGrowth>();
        }

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
        if (playerAnimator != null)
        {
            playerAnimator.ResetPestisideUpgrades(false);
        }

        if (helperNPC == null)
        {
            helperNPC = FindObjectOfType<HelperNPC>();
        }
        if (helperNPC != null)
        {
            helperNPC.ChangeFirstNight(false);
        }

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
        ResetPlayerPosition();

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
            coinCount -= dailyGoal;
            UpdateCoinUI();
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

        if (Shop != null) Shop.style.display = DisplayStyle.Flex;

        continueButton.clicked -= OnContinueButtonClicked;
        continueButton.clicked += OnContinueButtonClicked;

        void OnContinueButtonClicked()
        {
            endDayScreen.style.display = DisplayStyle.None;

            if (goalMet)
            {
                nightUI.style.display = DisplayStyle.Flex;
                GameBackground.style.display = DisplayStyle.None; 
            }
            else
            {
                nightUI.style.display = DisplayStyle.None;
                dayUI.style.display = DisplayStyle.None;
                objectivesScreen.style.display = DisplayStyle.None;
                if (settingsPanel != null)
                {
                    settingsPanel.style.display = DisplayStyle.None;
                }

                // Reset game 
                ResetPlayerPosition();
                ResetNPCPosition();
                ResetFlyPosition();

                GameBackground.style.display = DisplayStyle.Flex;
            }
        }

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



    //QUIT GAME BUTTON


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
        if (settingsPanel.style.display == DisplayStyle.None)
        {
            settingsPanel.style.display = DisplayStyle.Flex;
            settingsPanel.BringToFront(); // Ensures it appears above everything
        }
        else
        {
            settingsPanel.style.display = DisplayStyle.None;
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

    public int GetDays()
    {
        return dayCount;
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
        dayCount = 0;
        isGamePaused = false;
        Time.timeScale = 1;

        //reseting progressions
        dayProgression.ResetProgression();
        flyAI.ResetFlyHealth();

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

        if (helperNPC != null)
        {
            helperNPC.ResetHelper(false);
        }

        ResetPlayerPosition();
        ResetNPCPosition();
        ResetFlyPosition();
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

    private void ResetNPCPosition()
    {
        GameObject flyies = GameObject.FindGameObjectWithTag("FlySwarm");
        GameObject flySpawn = GameObject.FindGameObjectWithTag("Fly-Spawn");

        if (flyies != null && flySpawn != null)
        {
            flyies.transform.position = flySpawn.transform.position;
            flyies.transform.rotation = flySpawn.transform.rotation;
        }
    }

    private void ResetFlyPosition()
    {
        GameObject helper = GameObject.FindGameObjectWithTag("Helper");
        GameObject helperOrigin = GameObject.FindGameObjectWithTag("Helper-Origin");

        if (helper != null && helperOrigin != null)
        {
            helper.transform.position = helperOrigin.transform.position;
            helper.transform.rotation = helperOrigin.transform.rotation;
        }
    }
}