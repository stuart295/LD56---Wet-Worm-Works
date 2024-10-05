using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BristleWorm : Worm
{
    [Header("Bristle Worm")]
    public float nutrientAbsorbRate = 0.1f;


    protected override Creature GetNearestPrey()
    {
        return GameManager.Instance.GetNearestCorpose(transform.position);
    }

    protected override void HuntPrey()
    {
        if (attachedToObject) return;

        base.HuntPrey();
    }


    protected override void OnReachedPrey()
    {
        if (prey == null) return;
        AttachToObject(prey.gameObject);
    }

    public override void Kill(bool leaveCorpse)
    {

        DetachFromObject();
        base.Kill(leaveCorpse);
    }

    public override void Update()
    {
        base.Update();

        UpdateEat();
    }

    private void UpdateEat()
    {
        if (!attachedToObject) return;
        if (prey == null)
        {
            OnPreyLost();
            return;
        }

        float nutrGained = prey.AbsorbNutrients(nutrientAbsorbRate*Time.deltaTime);
        foodLevel = Mathf.Clamp01(foodLevel + nutrGained);
    }

    protected override void OnPreyLost()
    {
        base.OnPreyLost();

        if (attachedToObject)
        {
            DetachFromObject();
        }
    }



}
