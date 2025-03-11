using Unity.Entities;
using UnityEngine;

//do not autogenerate these: https://discussions.unity.com/t/generateauthoringcomponent-in-1-0/895783/5
//but cannot find the generic verison, perhaps make a custom one
class GhostTagAuthoring : MonoBehaviour
{
}
class GhostTagBaker : Baker<GhostTagAuthoring>
{
    public override void Bake(GhostTagAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new GhostTagComponent());
    }
}
