using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI_ComboBar : MonoBehaviour
{
    public TMPro.TMP_Text ComboText;
    public Slider ComboSlider;

    private float3 originalScale;

    private bool hidden = true;

    void Toggle(bool show)
    {
        hidden = !show;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    void Start()
    {
        originalScale = transform.localScale;

        Toggle(false);

        LeanTween.scale(gameObject, originalScale * 1.1f, 1.0f).setLoopPingPong().setEaseInOutSine();

        GameManager.Instance.LevelManager.LevelSetUp += (level, level_rank) =>
        {
            Toggle(false);
        };

        GameManager.Instance.Scores.OnComboAdded += (combo) =>
        {
            Toggle(true);

            LeanTween.scale(ComboText.gameObject, Vector3.one * 1.25f, 0.2f).setEaseOutBack().setOnComplete(() =>
            {
                LeanTween.scale(ComboText.gameObject, Vector3.one, 0.2f).setEaseOutBack();
            });


            var color = Color.Lerp(Color.green, Color.red, GameManager.Instance.Scores.Combo / 10.0f);
            ComboText.text = "x" + GameManager.Instance.Scores.Combo.ToString();
            ComboSlider.value = GameManager.Instance.Scores.GetComboProgress();
            ComboText.color = color;
        };

        GameManager.Instance.Scores.OnComboEnded += (combo) =>
        {
            Toggle(false);
        };
    }

    void Update()
    {
        if (!hidden)
        {
            ComboSlider.value = GameManager.Instance.Scores.GetComboProgress();
        }
    }
}

