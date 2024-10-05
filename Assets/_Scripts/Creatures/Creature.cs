using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Creature : MonoBehaviour
{

    public CreatureDefinition def;

    [Header("Live stats")]
    public float foodLevel = 0.5f;
    public float lifespanCur = 0f;


    private float nextReproductionTime = 0f;
    protected Rigidbody2D rb;
    protected bool dead = false;
    protected Creature prey;

    public virtual void Reproduce()
    {
        float offsetRange = def.offspringSpawnOffsetDist;

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetRange, offsetRange), UnityEngine.Random.Range(-offsetRange, offsetRange), 0);
        GameManager.Instance.SpawnCreature(def, transform.position + offset, def.offspringCount);
    }


    protected virtual void Awake()
    {
        SetNextReproductionTime();
        rb = GetComponent<Rigidbody2D>();
    }

    private void SetNextReproductionTime()
    {
        nextReproductionTime += (def.reproductionIntervalNorm +Random.Range(-def.reproductionIntervalNoise, def.reproductionIntervalNoise) ) * def.maxLifespanSeconds;
    }

    public virtual void Update()
    {
        UpdateFoodLevels();
        UpdateLifespan();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.zero;
        UpdateAI();
    }

    protected virtual void UpdateAI()
    {
        //Implement in children, if they think...
    }

    private void UpdateLifespan()
    {
        lifespanCur += Time.deltaTime;
        if (lifespanCur >= def.maxLifespanSeconds)
        {
            Kill(leaveCorpse: true);
        }else if (lifespanCur >= nextReproductionTime)
        {
            Reproduce();
            SetNextReproductionTime();
        }
    }


    protected virtual void UpdateFoodLevels()
    {
        foodLevel  = Mathf.Clamp01(foodLevel -= def.hungerRate*Time.deltaTime);
        if (foodLevel <= 0)
        {
            Kill(leaveCorpse: true);
        }
    }

    public void Kill(bool leaveCorpse)
    {
        dead = true;

        if (leaveCorpse)
        {
            //TODO
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager.Instance.OnCreatureDeath(this, leaveCorpse);
        
    }

    protected Creature GetNearestPrey()
    {
        if (def.speciesToEat == null || def.speciesToEat.Count == 0) return null;

        foreach (CreatureDefinition preyDef in def.speciesToEat)
        {
            Creature prey = GameManager.Instance.GetNearestCreature(preyDef, transform.position);
            if (prey != null) return prey;
        }

        return null;
    }

    protected bool Eat(Creature creature)
    {
        if (creature == null) return false;
        if (creature.dead) return false;

        foodLevel = creature.def.nutrition ;
        creature.Kill(leaveCorpse: false);


        return true;
    }
}
