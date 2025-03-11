using Unity.Entities;
using UnityEngine;

public struct HitObjectComponent : IComponentData
{
    public RaycastHit raycastHit;
    public bool isCube;
    public bool isOverlapping;
    public int hitBlockId;
}
