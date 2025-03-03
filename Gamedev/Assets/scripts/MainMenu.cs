using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour {
    [SerializeField] UIDocument mainMenuDocument;

    private Button startButton;
    private Button settingsButton;
    private Button quitButton;

    private void Awake() {
        VisualElement root = mainMenuDocument.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        startButton.clickable.clicked += OnStartButton;
        settingsButton.clickable.clicked += OnSettingsButton;
        quitButton.clickable.clicked += OnQuitButton;
    }

    private void OnStartButton() {
        SceneManager.LoadScene("GameScene");
    }

    private void OnSettingsButton() {
        SceneManager.LoadScene("SettingsScene");
    }
    private void OnQuitButton() {
        Application.Quit();
    }

}
