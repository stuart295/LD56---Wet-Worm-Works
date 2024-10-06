using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyInfoUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text stats;

    [Header("Settings")]
    public Vector2 offset = Vector2.zero;


    public void Show(CreatureDefinition def)
    {
        gameObject.SetActive(true);
        title.text = def.creatureName;
        description.text = def.description;

        string statsString = "";
        string habitat = def.livesAtSurface ? "Surface" : "Depths";

        statsString += $"Habitat: {habitat}";
        statsString += $"\nLifespan: {def.maxLifespanSeconds}s";
        statsString += $"\nOffspring: {Mathf.FloorToInt(1f/def.reproductionIntervalNorm)}";
        statsString += $"\nMax biomass: {def.maxNutrients}";
        statsString += $"\nCorpse biomass multiplier: {def.nutrientDeathMultiplier}";

        stats.text = statsString;


    }

    public void Hide()
    {
        gameObject.SetActive(false);    
    }


    private void Update()
    {
        if (!gameObject.activeSelf) return;

        Vector3 mousePosition = Input.mousePosition; // Get the mouse position in screen space
        GetComponent<RectTransform>().anchoredPosition = new Vector2(mousePosition.x, mousePosition.y) + offset;
    }

}
