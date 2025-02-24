using Unity.Entities;
using UnityEngine;

class CursorMeshTagAuthoring : MonoBehaviour
{
}
class CursorMeshTagBaker : Baker<CursorMeshTagAuthoring>
{
    public override void Bake(CursorMeshTagAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new CursorMeshTagComponent());
    }
}
