using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddWormButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Image cooldownImage;
    private Button button;

    private CreatureDefinition creatureDef;

    public void Setup(CreatureDefinition def, Action<CreatureDefinition> onClick)
    {
        icon.sprite = def.icon;
        button = GetComponent<Button>();

        button.onClick.AddListener(() => onClick(def));
        SetCooldown(0f);
        creatureDef = def;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.buyInfo.Show(creatureDef);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.buyInfo.Hide();
    }
}
