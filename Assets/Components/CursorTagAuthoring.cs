using Unity.Entities;
using UnityEngine;

class CursorTagAuthoring : MonoBehaviour
{
}
class CursorTagBaker : Baker<CursorTagAuthoring>
{
    public override void Bake(CursorTagAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new CursorTagComponent());
    }
}
