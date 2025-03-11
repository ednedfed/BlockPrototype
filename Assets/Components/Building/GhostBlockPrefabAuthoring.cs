using Unity.Entities;
using UnityEngine;

class GhostBlockPrefabAuthoring : MonoBehaviour
{
}
class GhostBlockPrefabBaker : Baker<GhostBlockPrefabAuthoring>
{
    public override void Bake(GhostBlockPrefabAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        //crazy naming convention here
        AddComponentObject(entity, new GhostBlockPrefabComponent());
    }
}
