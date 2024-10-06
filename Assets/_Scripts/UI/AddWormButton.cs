using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWormButton : MonoBehaviour
{
    public Image icon;
    public Image cooldownImage;
    private Button button;
    

    public void Setup(CreatureDefinition def, Action<CreatureDefinition> onClick)
    {
        icon.sprite = def.icon;
        button = GetComponent<Button>();

        button.onClick.AddListener(() => onClick(def));
        SetCooldown(0f);
    }

    public void SetCooldown(float percent)
    {
        if (percent == 0)
        {
            cooldownImage.gameObject.SetActive(false);
            button.interactable = true;
        }
        else
        {
            cooldownImage.gameObject.SetActive(true);
            cooldownImage.fillAmount = percent;
            button.interactable = false;
        }
    }
}
