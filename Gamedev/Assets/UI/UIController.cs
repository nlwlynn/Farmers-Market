using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;


public class UIController : MonoBehaviour
{
    public VisualElement ui;

    public Button Harvest;
    public Button Spray;
    public Button Move;
    public Button Shop;
    public Button PlayPause;
    public Button Settings;
    public Button Coins;
    public Button Cash;
    
    //for playpause button
    private bool isGamePaused = false;

    //for settings button
    public VisualElement settingsPanel;

    //for Progress bar for Phases---------------------------------------------------------------------------------
    public ProgressBar phaseTimer;
    private float timerDuration = 100f; //5min
    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    //brightness slider
    public Slider brightnessSlider;

    //volume slider
    public Slider volumeSlider;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        
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

    }

    private void OnEnable()
    {

        //bottomContainer Buttons
        Harvest = ui.Q<Button>("Harvest");
        if (Harvest != null)
        {
            Harvest.clicked += OnHarvestButtonClicked;
        }

        Spray = ui.Q<Button>("Spray");
        if (Spray != null)
        {
            Spray.clicked += OnSprayButtonClicked;
        }

        Move = ui.Q<Button>("Move");
        if (Move != null)
        {
            Move.clicked += OnMoveButtonClicked;
        }

        //SideBar Buttons

        Shop = ui.Q<Button>("Shop");
        if (Shop != null)
        {
            Shop.clicked += OnShopButtonClicked;
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

        //topContainer Buttons
        Coins = ui.Q<Button>("Coins");
        if (Coins != null)
        {
            Coins.clicked += OnCoinsButtonClicked;
        }

        Cash = ui.Q<Button>("Cash");
        if (Cash != null)
        {
            Cash.clicked += OnCashButtonClicked;
        }


    }



    private void Update()
    {
        // running and the game is not paused
        if (isTimerRunning && !isGamePaused)
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
        Debug.Log("Shop Button Clicked");
    }


    //PAUSE AND PLAY GAME
    private void OnPlayPauseButtonClicked()
    {
        Debug.Log("PlayPause Button Clicked");

        isGamePaused = !isGamePaused;

        if(isGamePaused)
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
        Coins.SetEnabled(isEnabled);
        Cash.SetEnabled(isEnabled);
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



    private void OnCoinsButtonClicked()
    {
        Debug.Log("Coins Button Clicked");
    }

    private void OnCashButtonClicked()
    {
        Debug.Log("Cash Button Clicked");
    }

}
