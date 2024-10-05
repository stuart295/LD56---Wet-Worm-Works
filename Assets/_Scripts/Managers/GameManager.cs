using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Creatures")]
    public CreatureDefinition algaeDef;


    [Header("Spawn points")]
    public List<Transform> algaeSpawns;

    public static GameManager Instance { get => instance; }

    private Dictionary<CreatureDefinition, int> creatureCounts = new Dictionary<CreatureDefinition, int>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SpawnThings();
    }

    private void SpawnThings()
    {
        foreach (Transform spawnPoint in algaeSpawns)
        {
            SpawnCreature(algaeDef, spawnPoint.position, 1);
        }
    }

    public void SpawnCreature(CreatureDefinition definition, Vector3 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int creatureCount = creatureCounts.GetValueOrDefault(definition, 0);
            if (creatureCount > definition.maxCreatures) return;

            Creature creature = Instantiate(definition.prefab, pos, Quaternion.identity).GetComponent<Creature>();

            if (creatureCounts.ContainsKey(definition))
            {
                creatureCounts[definition] += 1;
            }
            else
            {
                creatureCounts.Add(definition, 1);
            }
        }

    }

    public void OnCreatureDeath(CreatureDefinition creatureDef)
    {
        if (creatureCounts.ContainsKey(creatureDef))
        {
            creatureCounts[creatureDef] -= 1;
        }
    }
}
