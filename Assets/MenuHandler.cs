using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class menuHandler : MonoBehaviour
{

    private Button _startButton;
    private Button _howToButton;
    private Button _exitButton;
    private Button _backButton;
    private Label _score;

    private VisualElement _mainContainer;
    private VisualElement _howToContainer;



    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) ToggleSettings();

    }

    private void OnEnable()
    {

        var uiDoc = GetComponent<UIDocument>();
        _mainContainer = uiDoc.rootVisualElement.Q("MainContainer") as VisualElement;
        _howToContainer = uiDoc.rootVisualElement.Q("HowToContainer") as VisualElement;
        _mainContainer.style.opacity = 1;
        _howToContainer.style.opacity = 0;

        _startButton = uiDoc.rootVisualElement.Q("StartButton") as Button;
        _howToButton = uiDoc.rootVisualElement.Q("HowToButton") as Button;
        _exitButton = uiDoc.rootVisualElement.Q("ExitButton") as Button;
        _backButton = uiDoc.rootVisualElement.Q("BackButton") as Button;

        _score = uiDoc.rootVisualElement.Q("Score") as Label;



        _startButton.RegisterCallback<ClickEvent>(evt => PrintClickMessage());
        _howToButton.RegisterCallback<ClickEvent>(evt => ToggleSettings());
        _exitButton.RegisterCallback<ClickEvent>(evt => QuitGame());

    }

    private void PrintClickMessage()
    {
        Debug.Log($"{"button"} was clicked!");
        GameManager.Instance.SceneLoader.LoadScene(SceneLoader.GAME_SCENE);
    }

    private void RestartGame()
    {
        Debug.Log($"{"button"} was clicked!");
        GameManager.Instance.SceneLoader.LoadScene(SceneLoader.GAME_SCENE);
    }


    private void ToggleSettings()
    {
        Debug.Log("Toggled HowTo!");

        if (_mainContainer.style.display != DisplayStyle.None)
        {
            _mainContainer.style.opacity = 0;
            _mainContainer.style.visibility = Visibility.Hidden;
            _mainContainer.style.display = DisplayStyle.None;
            _howToContainer.style.display = DisplayStyle.Flex;
            _howToContainer.style.visibility = Visibility.Visible;
            _howToContainer.style.opacity = 1;

            Debug.Log("got this far");
        }
        else
        {
            _howToContainer.style.opacity = 0;
            _howToContainer.style.visibility = Visibility.Hidden;
            _howToContainer.style.display = DisplayStyle.None;
            _mainContainer.style.display = DisplayStyle.Flex;
            _mainContainer.style.visibility = Visibility.Visible;
            _mainContainer.style.opacity = 1;

        }

    }
    private void QuitGame()
    {
        Debug.Log("Quit game!");
    }

}
