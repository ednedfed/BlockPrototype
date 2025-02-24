using Unity.Entities;
using UnityEngine;

//do not autogenerate these: https://discussions.unity.com/t/generateauthoringcomponent-in-1-0/895783/5
//but cannot find the generic verison, perhaps make a custom one
class HitObjectAuthoring : MonoBehaviour
{
}
class HitObjectBaker : Baker<HitObjectAuthoring>
{
    public override void Bake(HitObjectAuthoring authoring)
    {
        //must be dynamic for local transform component to exist?
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new HitObjectComponent());
    }
}
