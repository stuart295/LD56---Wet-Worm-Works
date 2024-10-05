using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leech : Worm
{
    [Header("Leech")]
    public float lifetimeDrainPerSec = 1.0f;


    protected override void OnReachedPrey()
    {
        if (Dead) return;
        if (prey == null || prey.Dead) return;
        AttachToObject(prey.gameObject);
    }



    protected override void HuntPrey()
    {
        if (attachedToObject) return;

        base.HuntPrey();
    }

    public override void Update()
    {
        base.Update();

        UpdateSuck();
    }

    private void UpdateSuck()
    {
        if (dead) return;
        if (!attachedToObject) return;
        if (prey == null || prey.Dead) return;

        prey.lifespanPenaltySecs += lifetimeDrainPerSec * Time.deltaTime;
        Nutrients = def.maxNutrients;
    }

    protected override void OnPreyLost()
    {
        base.OnPreyLost();

        if (attachedToObject)
        {
            DetachFromObject();
        }
    }

    public override float Kill(bool leaveCorpse)
    {
        DetachFromObject();
        return base.Kill(leaveCorpse);
    }
}
