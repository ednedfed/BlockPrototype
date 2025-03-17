using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BattlingContext : MonoBehaviour
{
    //scene objects, make spawn later
    //public GameObject character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SpawnPointComponent[] spawnPoints = Object.FindObjectsByType<SpawnPointComponent>(FindObjectsSortMode.None);

        //todo: totally custom world to exclude unnecessary sytems?
        var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
        var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        var transformGroup = world.GetOrCreateSystemManaged<TransformSystemGroup>();

        var blockTypes = Resources.Load<BlockTypes>("ScriptableObjects/BlockTypes");

        RigidbodyEntityFactory rigidbodyEntityFactory = new RigidbodyEntityFactory();
        PlacedBlockContainer placedBlockContainer = new PlacedBlockContainer();
        BlockGameObjectContainer blockGameObjectContainer = new BlockGameObjectContainer(blockTypes, false);

        BlockFactory blockFactory = new BlockFactory(blockTypes);
        blockFactory.RegisterBlockListener(placedBlockContainer);
        blockFactory.RegisterBlockListener(blockGameObjectContainer);

        Dictionary<int, Entity> entityPerBlockId = new Dictionary<int, Entity>();

        //every block must have this from now on
        RegisterBlockEntityBuilder(world, simulationGroup, blockFactory, entityPerBlockId, new BlockEntityBuilder(entityPerBlockId));
        RegisterBlockEntityBuilder(world, simulationGroup, blockFactory, entityPerBlockId, new WheelEntityBuilder(entityPerBlockId));
        RegisterBlockEntityBuilder(world, simulationGroup, blockFactory, entityPerBlockId, new LaserEntityBuilder(entityPerBlockId));

        AddToWorldAndGroupSystemManaged(new PlaceBlockSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new LoadAtStartSystem(blockFactory, spawnPoints), world, simulationGroup);
        
        AddToWorldAndGroupSystemManaged(new ParentGameObjectToMachineSystem(blockGameObjectContainer), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new ParentCameraToMachineSystem(Camera.main.transform.parent), world, simulationGroup);

        AddToWorldAndGroupSystemManaged(new WeaponAimRaycastSystem(Camera.main.transform), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new LaserShootingSystem(blockGameObjectContainer), world, simulationGroup);
        

        //camera has no parent yet in this context
        AddToWorldAndGroupSystemManaged(new MouseLookRotationSystem(Camera.main.transform), world, simulationGroup);

        var fixedStepSimulationGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new CreateCompositeCollisionSystem(blockGameObjectContainer, placedBlockContainer, rigidbodyEntityFactory, spawnPoints, entityPerBlockId),
            world, fixedStepSimulationGroup);        

        AddToWorldAndGroupSystemManaged(new MachineControllerSystem(), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new WheelsApplyForcesSystem(), world, fixedStepSimulationGroup);

        simulationGroup.SortSystems();
        fixedStepSimulationGroup.SortSystems();
    }

    static void RegisterBlockEntityBuilder<T>(World world, SimulationSystemGroup simulationGroup, BlockFactory blockFactory, Dictionary<int, Entity> entityPerBlockId, T builder)
         where T : BlockEntityBuilder
    {
        blockFactory.RegisterBlockListenerWithCategory(builder);
        AddToWorldAndGroupSystemManaged(builder, world, simulationGroup);
    }

    static void AddToWorldAndGroupSystemManaged<T>(T system, World w, ComponentSystemGroup simulationGroup) where T : ComponentSystemBase
    {
        var addedSystem = w.AddSystemManaged(system);
        simulationGroup.AddSystemToUpdateList(addedSystem);
    }
}
