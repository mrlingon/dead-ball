using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DragPowerIndicator : MonoBehaviour
{
    Slider Slider;
    TMPro.TMP_Text Text;

    void Start()
    {
        TryGetComponent(out Slider);
        Text = GetComponentInChildren<TMPro.TMP_Text>();
    }

    void Update()
    {
        if (GameManager.Instance.Player?.DragPower)
        {
            var perc = GameManager.Instance.Player.DragPower.PowerUsage / GameManager.Instance.Player.DragPower.MaxPower;
            Slider.value = perc;
            Text.text = $"{(int)(perc * 100)}%";
        }
    }
}
