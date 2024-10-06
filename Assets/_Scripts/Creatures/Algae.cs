using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algae : Creature
{

    public float correctiveForceVertical = 5f;
    public float correctiveForceCenter = 1f;

    protected override bool LeavesCorpseOnDeath => false;

    protected override void Awake()
    {
        base.Awake();

        rb.MoveRotation(Random.Range(0, 360));
    }

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
            Vector2 force = Vector2.up * correctiveForceVertical;
            rb.AddForce(force);
        }

        Vector2 centerDir = GameManager.Instance.wanderBoundsUpper.bounds.center - transform.position;
        rb.AddForce(centerDir.normalized * correctiveForceCenter);

    }
}
