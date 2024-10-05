using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leech : Worm
{
    [Header("Leech")]
    public float lifetimeDrainPerSec = 1.0f;
    public SpringJoint2D attachedJoint;

    private bool attachedToPrey = false;



    protected override void OnReachedPrey()
    {
        if (prey == null || prey.Dead) return;

        attachedToPrey = true;
        attachedJoint.enabled = true;
        attachedJoint.connectedBody = prey.GetComponent<Rigidbody2D>();
        rb.freezeRotation = false;
    }

    protected override void HuntPrey()
    {
        if (attachedToPrey) return;

        base.HuntPrey();
    }

    public override void Update()
    {
        base.Update();

        UpdateSuck();
    }

    private void UpdateSuck()
    {
        if (!attachedToPrey) return;
        if (prey == null || prey.Dead) return;

        prey.lifespanPenaltySecs += lifetimeDrainPerSec * Time.deltaTime;
        foodLevel = 1f;
    }

    protected override void OnPreyDead()
    {
        base.OnPreyDead();

        if (attachedToPrey)
        {
            DetachFromPrey();
        }
    }

    private void DetachFromPrey()
    {
        attachedJoint.enabled = false;
        rb.freezeRotation = true;
        attachedToPrey = false;
    }
}
