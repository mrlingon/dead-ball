using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTemp : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) GameManager.Instance.SceneLoader.LoadScene(SceneLoader.GAME_SCENE);
    }
}
