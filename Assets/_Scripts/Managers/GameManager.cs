using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Creatures")]
    public CreatureDefinition algaeDef;


    [Header("Spawns")]
    public List<Transform> algaeSpawns;
    public int algaeStartCount = 10;

    public Collider2D bounds;

    public static GameManager Instance { get => instance; }

    private Dictionary<CreatureDefinition, int> creatureCounts = new Dictionary<CreatureDefinition, int>();
    private Dictionary<CreatureDefinition, HashSet<Creature>> creatureMap = new Dictionary<CreatureDefinition, HashSet<Creature>>();

    private HashSet<Creature> corpses =  new HashSet<Creature>();

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
            for (int i = 0; i < algaeStartCount; i++)
            {
                Vector3 offset = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                SpawnCreature(algaeDef, spawnPoint.position + offset, 1);
            }
            
        }
    }

    public void SpawnCreature(CreatureDefinition definition, Vector3 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int creatureCount = creatureCounts.GetValueOrDefault(definition, 0);
            if (creatureCount > definition.maxCreatures) return;

            Creature creature = Instantiate(definition.prefab, pos, Quaternion.identity).GetComponent<Creature>();

            creatureCounts[definition] = creatureCounts.ContainsKey(definition) ? creatureCounts[definition] + 1 : 1;

            if (creatureMap.ContainsKey(definition)) {
                creatureMap[definition].Add(creature);
            }
            else
            {
                creatureMap[definition] = new HashSet<Creature> { creature };
            }
        }

    }

    public void OnCreatureDeath(Creature creature, bool corpseLeft)
    {
        if (creatureCounts.ContainsKey(creature.def))
        {
            creatureCounts[creature.def] -= 1;
        }

        if (creatureMap.ContainsKey(creature.def))
        {
            if (corpseLeft)
            {
                corpses.Add(creature);
            }

            creatureMap[creature.def].Remove(creature);
        }


    }

    //Brute force fun
    public Creature GetNearestCreature(CreatureDefinition targetDefinition, Vector3 position)
    {
        HashSet<Creature> creatures = creatureMap.GetValueOrDefault(targetDefinition, new HashSet<Creature>());
        return creatures.OrderBy(c => Vector3.Distance(c.transform.position, position)).FirstOrDefault();
    }

    public Vector2 ConstrainToBounds(Vector2 position)
    {
        Bounds bounds = this.bounds.bounds;
        float clampedX = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        return new Vector2(clampedX, clampedY);
    }
}
