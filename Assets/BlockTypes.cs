using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockTypes", menuName = "Scriptable Objects/BlockTypes")]
public class BlockTypes : ScriptableObject
{
    public BlockTypesData[] blockDatas;
}

[Serializable]
public struct BlockTypesData
{
    public BlockCategory blockCategory;
    public GameObject buildPrefab;
    public GameObject ghostPrefab;
    public GameObject simulationPrefab;
}

public enum BlockCategory
{ 
    Primitive,
    Wheel,
    Laser,
}