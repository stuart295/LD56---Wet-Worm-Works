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
    


    public virtual void Reproduce()
    {
        float offsetRange = def.offspringSpawnOffsetDist;

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetRange, offsetRange), UnityEngine.Random.Range(-offsetRange, offsetRange), 0);
        GameManager.Instance.SpawnCreature(def, transform.position + offset, def.offspringCount);
    }


    protected virtual void Awake()
    {
        SetNextReproductionTime();
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

    private void UpdateLifespan()
    {
        lifespanCur += Time.deltaTime;
        if (lifespanCur >= def.maxLifespanSeconds)
        {
            Kill();
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
            Kill();
        }
    }

    public void Kill()
    {
        GameManager.Instance.OnCreatureDeath(def);
        Destroy(gameObject);
    }
}
