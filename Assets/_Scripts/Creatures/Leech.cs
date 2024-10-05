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
        if (!attachedToObject) return;
        if (prey == null || prey.Dead) return;

        prey.lifespanPenaltySecs += lifetimeDrainPerSec * Time.deltaTime;
        foodLevel = 1f;
    }

    protected override void OnPreyLost()
    {
        base.OnPreyLost();

        if (attachedToObject)
        {
            DetachFromObject();
        }
    }

    public override void Kill(bool leaveCorpse)
    {

        DetachFromObject();
        base.Kill(leaveCorpse);
    }
}
