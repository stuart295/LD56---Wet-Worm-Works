using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BristleWorm : Worm
{
    [Header("Bristle Worm")]
    public float nutrientAbsorbRate = 0.1f;

    public CreatureDefinition nutrientsPrefab;
    public int nutrientCount = 1;
    public float nutrientPoopThreshold = 60f;
    public float nutrientPoopUse = 50f;

    protected override Creature GetNearestPrey()
    {
        return GameManager.Instance.GetNearestCorpose(transform.position);
    }

    protected override void HuntPrey()
    {
        if (attachedToObject) return;

        base.HuntPrey();
    }


    protected override void OnReachedPrey()
    {
        if (prey == null) return;
        AttachToObject(prey.gameObject);
    }

    public override float Kill(bool leaveCorpse)
    {
        DetachFromObject();
        return base.Kill(leaveCorpse);
    }

    public override void Update()
    {
        base.Update();

        UpdateEat();
    }

    private void UpdateEat()
    {
        if (!attachedToObject) return;
        if (prey == null)
        {
            OnPreyLost();
            return;
        }

        Vector2 preyPos = prey.transform.position;

        float nutrGained = prey.AbsorbNutrients(nutrientAbsorbRate*Time.deltaTime);
        Nutrients += nutrGained;

        if (Nutrients >= nutrientPoopThreshold)
        {
            Nutrients -= nutrientPoopUse;
            GameManager.Instance.SpawnCreature(nutrientsPrefab, transform.position, nutrientCount);
        }

        if (prey == null || prey.corpseNutrients <= 0)
        {
            OnPreyLost();
        }
    }

    protected override void OnPreyLost()
    {
        base.OnPreyLost();

        if (attachedToObject)
        {
            DetachFromObject();
        }
    }



}
