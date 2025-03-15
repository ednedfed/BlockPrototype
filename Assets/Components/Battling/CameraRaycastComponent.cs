using Unity.Entities;
using UnityEngine;

public struct CameraRaycastComponent : IComponentData
{
    internal RaycastHit hitInfo;
}
