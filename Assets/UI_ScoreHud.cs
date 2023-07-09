using UnityEngine;

public class UI_ScoreHud : MonoBehaviour
{
    public TMPro.TMP_Text Text;

    void Start()
    {
        Text.text = "0";
    }

    void Update()
    {
        if (GameManager.Instance?.Scores != null)
        {
            Text.text = GameManager.Instance.Scores.Score.ToString();
        }
    }
}
