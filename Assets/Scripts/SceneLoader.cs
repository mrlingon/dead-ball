using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public float TransitionTime;


    public readonly static int MENU_SCENE = 0;
    public readonly static int GAME_SCENE = 1;
    public readonly static int GAMEOVER_SCENE = 2;
    public readonly static int GAMEWON_SCENE = 3;

    public bool DontLoadMenuOnStart = false;

    public void Awake()
    {
        if (GameManager.Instance.Scores != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.SceneLoader = this;

        if (!DontLoadMenuOnStart)
            LoadScene(MENU_SCENE);
    }


    /*IEnumerator LoadScene(int sceneIndex, int unloadScene, bool music = true)
    {
        yield return new WaitForSeconds(TransitionTime);
        var operation = SceneManager.UnloadSceneAsync(unloadScene);
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.33f);

        yield return new WaitForSeconds(TransitionTime);
    }*/


    public void LoadScene(int targetScene)
    {
        GameManager.Instance.Pause();
        GameManager.Instance.Player?.ToggleControl(false);

        var asyncop = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        asyncop.completed += operation =>
        {
            GameManager.Instance.Resume();
            GameManager.Instance.Player?.ToggleControl(true);
        };
    }
}