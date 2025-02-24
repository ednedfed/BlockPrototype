using Unity.Entities;
using UnityEngine;

//do not autogenerate these: https://discussions.unity.com/t/generateauthoringcomponent-in-1-0/895783/5
//but cannot find the generic verison, perhaps make a custom one
class GhostBlockDataAuthoring : MonoBehaviour
{
}
class GhostBlockDataBaker : Baker<GhostBlockDataAuthoring>
{
    public override void Bake(GhostBlockDataAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new GhostBlockDataComponent());
    }
}
