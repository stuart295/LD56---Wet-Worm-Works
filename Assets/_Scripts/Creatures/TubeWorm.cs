using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TubeWorm : Worm
{
    [Header("Tube Worm")]

    public CreatureDefinition crystalDef;
    public int crystalCount = 1;
    public float nutrientPickupRange = 10f;
    public Transform poopPoint;

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
        if (Nutrients >= def.searchFoodThreshold) return;

        Creature nearestFood = GetNearestPrey();
        if (nearestFood == null) return;

        if (Vector2.Distance(transform.position, nearestFood.transform.position) > nutrientPickupRange) return;

        float nutrGained = nearestFood.Kill(leaveCorpse: false);
        Nutrients += nutrGained;

        GameManager.Instance.SpawnCreature(crystalDef, poopPoint.position, crystalCount);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attachedToObject || Dead) return;

        if (targetVent != null && collision.gameObject == targetVent.gameObject)
        {
            SulphurVent vent = collision.gameObject.GetComponent<SulphurVent>();
            if (vent == null) return;

            AttachToObject(vent.gameObject, collide: true);

        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject)) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, nutrientPickupRange);
    }
#endif

}
