using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Settings")]
    public float corpseDecayTimeSecs = 30f;
    public List<CreatureDefinition> addableCreatures;
   

    [Header("Spawns")]
    public List<CreatureSpawn> creatureSpawns;

    public Collider2D wanderBoundsUpper;
    public Collider2D wanderBoundsLower;

    [Header("Vents")]
    public List<SulphurVent> sulphurVents;

    [Header("UI")]
    public Transform wormBar;
    public GameObject wormBtnPrefab;
    public TMP_Text scoreText;

    public static GameManager Instance { get => instance; }

    private Dictionary<CreatureDefinition, int> creatureCounts = new Dictionary<CreatureDefinition, int>();
    private Dictionary<CreatureDefinition, HashSet<Creature>> creatureMap = new Dictionary<CreatureDefinition, HashSet<Creature>>();

    private HashSet<Creature> corpses =  new HashSet<Creature>();
    private List<AddWormButton> wormButtons;
    private float buyCooldown = 0f;
    private float buyCooldownMax = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            return;
        }

        instance = this;
        buyCooldownMax = 1f;
        SetupWormBar();
    }

    private void Update()
    {
        UpdateBuyCooldown();
        UpdateScore();
    }

    private void UpdateScore()
    {
        float score = CalcScore();
        score = Mathf.RoundToInt(score);
        scoreText.text = $"Biomass: {score}";
    }

    private float CalcScore()
    {
        return creatureMap.Values.ToList().Sum(c => c.Sum(x => x.Nutrients));
    }

    private void UpdateBuyCooldown()
    {
        buyCooldown -= Time.deltaTime;
        buyCooldown = Mathf.Max(buyCooldown, 0f);

        wormButtons.ForEach(b => b.SetCooldown(buyCooldown/ buyCooldownMax));
    }

    private void SetupWormBar()
    {
        wormButtons = new List<AddWormButton>();

        foreach (CreatureDefinition def in addableCreatures)
        {
            AddWormButton btn = Instantiate(wormBtnPrefab, wormBar).GetComponent<AddWormButton>();
            btn.Setup(def, OnAddWormClicked);
            wormButtons.Add(btn);
        }
    }

    private void OnAddWormClicked(CreatureDefinition definition)
    {
        Collider2D area = definition.livesAtSurface ? wanderBoundsUpper : wanderBoundsLower;
        Vector3 spawnPoint = GetRandomPointInArea(area);//TODO timer
        SpawnCreature(definition, spawnPoint, 1);

        buyCooldown = definition.buyCooldownSecs;
        buyCooldownMax = definition.buyCooldownSecs;
    }

    private Vector3 GetRandomPointInArea(Collider2D area)
    {
        Bounds bounds = area.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(randomX, randomY, 0f);
    }


    private void Start()
    {
        SpawnThings();
    }

    private void SpawnThings()
    {
        //Algae
        foreach (CreatureSpawn creatureSpawn in creatureSpawns)
        {
            for (int i = 0; i < creatureSpawn.count; i++)
            {
                Vector3 offset = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                SpawnCreature(creatureSpawn.definition, creatureSpawn.spawnPoint.position + offset, 1);
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

    public Vector2 ConstrainToBounds(Vector2 position, bool upper)
    {
        Bounds bounds = upper ? wanderBoundsUpper.bounds : wanderBoundsLower.bounds;
        float clampedX = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        return new Vector2(clampedX, clampedY);
    }

    public void RemoveCorpse(Creature creature)
    {
        if (!corpses.Contains(creature)) return;

        corpses.Remove(creature);
    }

    public Creature GetNearestCorpose(Vector3 position)
    {
        return corpses.OrderBy(c => Vector3.Distance(c.transform.position, position)).FirstOrDefault();
    }

    public SulphurVent GetRandomVent()
    {
        return sulphurVents.OrderBy(v => Random.value).FirstOrDefault();    
    }

    [Serializable]
    public class CreatureSpawn
    {
        public Transform spawnPoint;
        public CreatureDefinition definition;
        public int count = 1;
    }
}
