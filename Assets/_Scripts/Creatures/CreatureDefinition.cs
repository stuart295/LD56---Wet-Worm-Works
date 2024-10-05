using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CreatureDef", menuName ="Game/Creature definition")]
public class CreatureDefinition : ScriptableObject
{
    [Header("Info")]
    public string creatureName = "";

    [TextArea(3, 5)]
    public string description = "";

    [Header("Spawning")]
    public GameObject prefab;
    public int maxCreatures = 50;

    [Header("Hunger")]
    public float hungerRate = 0.1f;

    [Header("Reproduction")]
    public float reproductionIntervalNorm = 0.4f;
    public float reproductionIntervalNoise = 0.05f;
    public int offspringCount = 1;
    public float offspringSpawnOffsetDist = 1f;


    [Header("Lifespan")]
    public float maxLifespanSeconds = 30f;

}
