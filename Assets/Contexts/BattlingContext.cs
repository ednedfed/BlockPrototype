using Unity.Entities;
using UnityEngine;

public class BattlingContext : MonoBehaviour
{
    //scene objects, make spawn later
    //public GameObject character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var blockTypes = Resources.Load<BlockTypes>("ScriptableObjects/BlockTypes");

        PlacedBlockContainer placedBlockContainer = new PlacedBlockContainer();
        BlockGameObjectContainer blockGameObjectContainer = new BlockGameObjectContainer(blockTypes);

        BlockFactory blockFactory = new BlockFactory(blockTypes);
        blockFactory.RegisterBlockListener(placedBlockContainer);
        blockFactory.RegisterBlockListener(blockGameObjectContainer);

        //todo: totally custom world to exclude unnecessary sytems
        var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
        var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

        //todo: parent camera, make an entity to drive

        AddToWorldAndGroupSystemManaged(new PlaceBlockSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new LoadAtStartSystem(blockFactory), world, simulationGroup);

        var fixedStepSimulationGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

        //todo: insert apply force methods here to control vehicle
        AddToWorldAndGroupSystemManaged(new CreateCompositeCollisionSystem(blockGameObjectContainer, placedBlockContainer), world, fixedStepSimulationGroup);

        simulationGroup.SortSystems();
        fixedStepSimulationGroup.SortSystems();
    }

    static void AddToWorldAndGroupSystemManaged<T>(T system, World w, ComponentSystemGroup simulationGroup) where T : ComponentSystemBase
    {
        var addedSystem = w.AddSystemManaged(system);
        simulationGroup.AddSystemToUpdateList(addedSystem);
    }
}
