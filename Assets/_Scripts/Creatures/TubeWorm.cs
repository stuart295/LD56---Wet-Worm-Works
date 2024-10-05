using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeWorm : Worm
{
    [Header("Tube Worm")]

    public CreatureDefinition crystalPrefab;
    public int crystalCount = 1;

    private SulphurVent targetVent;

    protected override void UpdateAI()
    {
        if (Dead) return;

        anims.SetFloat(ANIM_SPEED, 0f);

        if (attachedToObject) return;

        if (targetVent == null)
        {
            targetVent = GameManager.Instance.GetRandomVent();
        }

        MoveTo(targetVent.transform.position);


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
        if (!attachedToObject || Dead) return;
        //if (prey == null)
        //{
        //    OnPreyLost();
        //    return;
        //}

        //Vector2 preyPos = prey.transform.position;

        //float nutrGained = prey.AbsorbNutrients(nutrientAbsorbRate*Time.deltaTime);
        //foodLevel = Mathf.Clamp01(foodLevel + nutrGained);

        //if (prey == null || prey.NutrientsCur <= 0)
        //{
        //    GameManager.Instance.SpawnCreature(nutrientsPrefab, preyPos, nutrientCount);
        //    OnPreyLost();
        //}
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attachedToObject || Dead) return;

        if (targetVent != null && collision.gameObject == targetVent.gameObject)
        {
            SulphurVent vent = collision.gameObject.GetComponent<SulphurVent>();
            if (vent == null) return;

            AttachToObject(vent.gameObject);

        }
    }


}
