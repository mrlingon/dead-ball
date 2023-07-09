using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class gameOverHandler : MonoBehaviour
{


    private Button _exitButton;
    private Button _restartButton;
    private Label _score;

    private VisualElement _mainContainer;



    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {

        var uiDoc = GetComponent<UIDocument>();


        _restartButton = uiDoc.rootVisualElement.Q("RestartButton") as Button;
        _exitButton = uiDoc.rootVisualElement.Q("ExitButton") as Button;

        _score = uiDoc.rootVisualElement.Q("Score") as Label;



        _restartButton.RegisterCallback<ClickEvent>(evt => RestartGame());
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





    private void QuitGame()
    {
        Debug.Log("Quit game!");
    }

}
