using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algae : Creature
{

    protected override bool LeavesCorpseOnDeath => false;

    protected override void UpdateNutrientLevels()
    {
        //Food never decreases 
    }

    protected override void Awake()
    {
        base.Awake();

        lifespanCur = Random.Range(0, 0.1f);
    }
}
