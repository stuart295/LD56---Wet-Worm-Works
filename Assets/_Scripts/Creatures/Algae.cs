using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algae : Creature
{

    protected override bool LeavesCorpseOnDeath => false;

    protected override void UpdateFoodLevels()
    {
        //Food never decreases 
    }



}
