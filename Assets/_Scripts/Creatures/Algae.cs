using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algae : Creature
{

    public float correctiveForce = 5f;

    protected override bool LeavesCorpseOnDeath => false;

    protected override void UpdateNutrientLevels()
    {
        //Food never decreases 
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 pos = rb.position;

        if (pos.y <= GameManager.Instance.boundaryHeight)
        {
            Vector2 force = Vector2.up * correctiveForce;
            rb.AddForce(force);
        }

    }
}
