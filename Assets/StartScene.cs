using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StartScene : MonoBehaviour
{
   [SerializeField]
   private float delayBeforeLoading = 10f;
      [SerializeField]
   private string sceneToLoad = "MainMenu";
   private float timeElapsed;

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed > delayBeforeLoading) {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
