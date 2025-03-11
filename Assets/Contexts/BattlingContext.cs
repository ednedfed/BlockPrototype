using Unity.Entities;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BattlingContext : MonoBehaviour
{
    //scene objects, make spawn later
    //public GameObject character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //todo: totally custom world to exclude unnecessary sytems?
        var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
        var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

        var blockTypes = Resources.Load<BlockTypes>("ScriptableObjects/BlockTypes");

        RigidbodyEntityFactory rigidbodyEntityFactory = new RigidbodyEntityFactory();
        PlacedBlockContainer placedBlockContainer = new PlacedBlockContainer();
        BlockGameObjectContainer blockGameObjectContainer = new BlockGameObjectContainer(blockTypes);

        BlockFactory blockFactory = new BlockFactory(blockTypes);
        blockFactory.RegisterBlockListener(placedBlockContainer);
        blockFactory.RegisterBlockListener(blockGameObjectContainer);

        RegisterBlockEntityBuilder<WheelEntityBuilder>(world, simulationGroup, blockFactory, 2);

        AddToWorldAndGroupSystemManaged(new PlaceBlockSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new LoadAtStartSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new ParentCameraToMachineSystem(), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new ParentGameObjectToMachineSystem(blockGameObjectContainer, placedBlockContainer), world, simulationGroup);
        

        //camera has no parent yet in this context
        AddToWorldAndGroupSystemManaged(new MouseLookRotationSystem(Camera.main.transform), world, simulationGroup);

        var fixedStepSimulationGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new CreateCompositeCollisionSystem(blockGameObjectContainer, placedBlockContainer, rigidbodyEntityFactory), world, fixedStepSimulationGroup);

        AddToWorldAndGroupSystemManaged(new MachineControllerSystem(), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new WheelsApplyForcesSystem(), world, fixedStepSimulationGroup);

        simulationGroup.SortSystems();
        fixedStepSimulationGroup.SortSystems();
    }

    static void RegisterBlockEntityBuilder<T>(World world, SimulationSystemGroup simulationGroup, BlockFactory blockFactory, uint blockType)
        where T : SystemBase, IBlockFactoryListener, new()
    {
        var builder = new T();
        blockFactory.RegisterBlockListener(blockType, builder);
        AddToWorldAndGroupSystemManaged(builder, world, simulationGroup);
    }

    static void AddToWorldAndGroupSystemManaged<T>(T system, World w, ComponentSystemGroup simulationGroup) where T : ComponentSystemBase
    {
        var addedSystem = w.AddSystemManaged(system);
        simulationGroup.AddSystemToUpdateList(addedSystem);
    }
}
