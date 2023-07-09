using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI_ReleaseBar : MonoBehaviour
{
    public Slider ComboSlider;
    public GameObject Image;

    private bool hidden = true;
    private float3 originalScale;

    void Toggle(bool show)
    {
        hidden = !show;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Toggle(false);
        originalScale = Image.transform.localScale;
        LeanTween.scale(Image, originalScale * 1.15f, 0.3f).setLoopPingPong().setEaseInOutSine();

        GameManager.Instance.OnCatchedBall += () =>
        {
            Toggle(true);
            ComboSlider.value = GameManager.Instance.Player.GetReleaseProgress();
        };

        GameManager.Instance.OnReleasedBall += () =>
        {
            Toggle(false);
        };

        GameManager.Instance.OnEnemyShootBall += () =>
        {
            Toggle(false);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!hidden)
        {
            ComboSlider.value = GameManager.Instance.Player.GetReleaseProgress();
        }
    }
}
