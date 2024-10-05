using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NutrientPellet : Creature
{

    public float horDriftForceMin = 1f;
    public float horDriftForceMax = 3f;


    protected float driftForce = 0f;

    protected override bool LeavesCorpseOnDeath => false;

    protected override void Awake()
    {
        base.Awake();
        driftForce = Random.Range(horDriftForceMin, horDriftForceMax);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateDrift();
    }

    private void UpdateDrift()
    {
        rb.AddForce(new Vector2(driftForce, 0));
    }
}
