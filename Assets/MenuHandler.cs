using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class menuHandler : MonoBehaviour
{

    private Button _startButton;
    private Button _settingsButton;
    private Button _exitButton;
    private Button _backButton;

    private VisualElement _mainContainer;
private VisualElement _settingsContainer;



    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable(){

        var uiDoc = GetComponent<UIDocument>();
        _mainContainer = uiDoc.rootVisualElement.Q("MainContainer") as VisualElement;
        _settingsContainer = uiDoc.rootVisualElement.Q("SettingsContainer") as VisualElement;
        _mainContainer.style.opacity = 1;
        _settingsContainer.style.opacity = 0;

        _startButton = uiDoc.rootVisualElement.Q("StartButton") as Button;
        _settingsButton = uiDoc.rootVisualElement.Q("SettingsButton") as Button;
        _exitButton = uiDoc.rootVisualElement.Q("ExitButton") as Button;
        _backButton = uiDoc.rootVisualElement.Q("BackButton") as Button;

        

        _startButton.RegisterCallback<ClickEvent>(evt => PrintClickMessage());
        _settingsButton.RegisterCallback<ClickEvent>(evt => ToggleSettings());
        _backButton.RegisterCallback<ClickEvent>(evt => ToggleSettings());
        _exitButton.RegisterCallback<ClickEvent>(evt => QuitGame());

    }

    private void PrintClickMessage() {

        Debug.Log($"{"button"} was clicked!");
        SceneManager.LoadScene(1);
      }


    private void ToggleSettings(){
        Debug.Log("Toggled settings!");

        if(_mainContainer.style.display != DisplayStyle.None) {
            _mainContainer.style.opacity = 0;
           _mainContainer.style.visibility = Visibility.Hidden;
           _mainContainer.style.display = DisplayStyle.None;
           _settingsContainer.style.display = DisplayStyle.Flex;
           _settingsContainer.style.visibility = Visibility.Visible;
           _settingsContainer.style.opacity = 1;

           Debug.Log("got this far");
        }
        else {
           _settingsContainer.style.opacity = 0;
           _settingsContainer.style.visibility = Visibility.Hidden;
           _settingsContainer.style.display = DisplayStyle.None;
            _mainContainer.style.display = DisplayStyle.Flex;
            _mainContainer.style.visibility = Visibility.Visible;
            _mainContainer.style.opacity = 1;

        }

    }
    private void QuitGame(){
            Debug.Log("Quit game!");
    }

}
