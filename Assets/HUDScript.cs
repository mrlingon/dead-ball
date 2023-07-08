using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUDScript : MonoBehaviour
{

    private Label score; 
        
    private Label kills; 
    private Label combo; 
    private VisualElement powerBar;
    private VisualElement comboBar;



    private int incrementer;



    // Start is called before the first frame update
    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        score = uiDoc.rootVisualElement.Q("Score") as Label;
        kills = uiDoc.rootVisualElement.Q("Kills") as Label;
        combo = uiDoc.rootVisualElement.Q("Combo") as Label;
        comboBar = uiDoc.rootVisualElement.Q("ComboBarValue") as VisualElement;
        powerBar = uiDoc.rootVisualElement.Q("PowerBarValue") as VisualElement;
    }

    // Update is called once per frame
    void Update()
    {
        score.text = incrementer.ToString();
        kills.text = incrementer.ToString();
        combo.text = incrementer.ToString();

        //det går att ändra powerbar och combobar genom att accessa deras width
        //vet inte om detta är mer nice än det som redan finns tho

        incrementer++;
    }

private void OnEnable(){

        

    }

}
