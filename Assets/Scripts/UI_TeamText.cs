using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Timers;
using Unity.Mathematics;
using UnityEngine;

public class UI_TeamText : MonoBehaviour
    {
    public TMPro.TMP_Text Team1Text;
    public TMPro.TMP_Text VsText;
    public TMPro.TMP_Text Team2Text;

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


    void Start()
    {
        originalScale = transform.localScale;

        Toggle(false);

        GameManager.Instance.LevelManager.LevelSetUp += (level, level_rank) =>
        {
            Team1Text.text = level.teamOne.teamName;
            Team2Text.text = level.teamTwo.teamName;

            Team1Text.color = level.teamOne.color;
            Team2Text.color = level.teamTwo.color;

            Team1Text.gameObject.SetActive(true);
            Team2Text.gameObject.SetActive(true);

            LeanTween.scale(Team1Text.gameObject, Vector2.one * 1.1f, 0.5f).setEaseInOutSine().setOnComplete(() =>
            {
                LeanTween.scale(Team1Text.gameObject, Vector2.one, 0.5f).setEaseInOutSine();
            });

            LeanTween.scale(Team2Text.gameObject, Vector2.one * 1.1f, 0.5f).setEaseInOutSine().setOnComplete(() =>
            {
                LeanTween.scale(Team2Text.gameObject, Vector2.one, 0.5f).setEaseInOutSine();
            });

            Timers.SetTimeout(500, () => {
                VsText.gameObject.SetActive(true);
            });

            // scale VsText.GameObject to 1.5f after 0.5s, and then scale back to 1
            LeanTween.scale(VsText.gameObject, Vector2.one * 1.5f, 0.5f).setDelay(0.5f).setEaseInOutSine().setOnComplete(() =>
            {
                LeanTween.scale(VsText.gameObject, Vector2.one, 0.5f).setEaseInOutSine();
            });
        };

        GameManager.Instance.LevelManager.LevelStart += (level, level_rank) =>
        {
            Toggle(false);
        };
    }
}
