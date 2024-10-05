using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Creature : MonoBehaviour
{

    public CreatureDefinition def;

    [Header("Live stats")]
    public float nutrientLevel = 0.5f;
    public float lifespanCur = 0f;
    public float lifespanPenaltySecs = 0f;
    public float corpseNutrients = 0f;


    private float nextReproductionTime = 0f;
    protected Rigidbody2D rb;
    protected bool dead = false;
    protected Creature prey;
    protected float deathTime = 0f;

    protected virtual bool LeavesCorpseOnDeath => true;
    protected float MaxLifespanSecs => def.maxLifespanSeconds - lifespanPenaltySecs;
    protected virtual bool FixVelocity => def.fixVelocity && !Dead;

    public virtual float Nutrients
    {
        get => nutrientLevel;
        set => nutrientLevel = Mathf.Clamp(value, 0, def.maxNutrients);
    }

    public bool Dead { get => dead; }

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
        Nutrients = def.startNutrients;
    }

    private void SetNextReproductionTime()
    {
        nextReproductionTime += (def.reproductionIntervalNorm +Random.Range(-def.reproductionIntervalNoise, def.reproductionIntervalNoise) ) * def.maxLifespanSeconds;
    }

    public virtual void Update()
    {
        if (dead)
        {
            UpdateDecay();
            return;
        }
        UpdateNutrientLevels();
        UpdateLifespan();
    }

    protected virtual void UpdateDecay()
    {
        if (Time.time - deathTime >= GameManager.Instance.corpseDecayTimeSecs)
        {
            GameManager.Instance.RemoveCorpse(this);
            Destroy(gameObject);//TODO effects?
        }
    }

    protected virtual void FixedUpdate()
    {
        if (dead) return;

        if (FixVelocity)
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
        if (lifespanCur >= MaxLifespanSecs)
        {
            Kill(leaveCorpse: LeavesCorpseOnDeath);
        }else if (lifespanCur >= nextReproductionTime)
        {
            Reproduce();
            SetNextReproductionTime();
        }
    }


    protected virtual void UpdateNutrientLevels()
    {
        Nutrients -= def.nutrientLossRate * Time.deltaTime;
        if (Nutrients <= 0)
        {
            Kill(leaveCorpse: LeavesCorpseOnDeath);
        }
    }

    public virtual float Kill(bool leaveCorpse)
    {
        if (dead) return 0;

        dead = true;
        deathTime = Time.time;

        corpseNutrients = Nutrients * def.nutrientDeathMultiplier;

        if (leaveCorpse)
        {
            rb.gravityScale = 1f;
            rb.freezeRotation = false;
            rb.drag = 3f;
            rb.angularDrag = 0.5f;
            
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager.Instance.OnCreatureDeath(this, leaveCorpse);
        return corpseNutrients;
    }

    protected virtual Creature GetNearestPrey()
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

        Nutrients += creature.Kill(leaveCorpse: false);
        
        return true;
    }

    public float AbsorbNutrients(float amount)
    {
        float amountActual = Mathf.Min(amount, corpseNutrients);

        corpseNutrients -= amount;

        if (corpseNutrients <= 0)
        {
            GameManager.Instance.RemoveCorpse(this);
            Destroy(gameObject);
        }

        return amountActual;
    }
}
