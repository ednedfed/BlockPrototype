using Unity.Entities;
using UnityEngine;

public partial class MainGameContext : MonoBehaviour
{
    //scene objects, make spawn later
    public GameObject character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //todo: also load static data?
        BlockTypes blockTypes = GameObject.FindFirstObjectByType<BlockTypes>();

        PlacedBlockContainer placedBlockContainer = new PlacedBlockContainer();
        BlockFactory blockFactory = new BlockFactory(blockTypes, placedBlockContainer);

        //todo: totally custom world to exclude unnecessary sytems
        var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
        var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new DeleteBlockSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostBlockTypeSyncSystem(blockTypes, placedBlockContainer), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostRotationUpdateSystem(), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new PlaceBlockSystem(blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new SaveLoadGameSystem(0, placedBlockContainer, blockFactory), world, simulationGroup);

        var fixedStepSimulationGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new FirstPersonControllerSystem(character.GetComponent<Rigidbody>(), character.transform), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostPositionUpdateSystem(character, placedBlockContainer), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostOverlappingSyncSystem(), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new ParentCursorToGhostSystem(), world, fixedStepSimulationGroup);

        simulationGroup.SortSystems();
        fixedStepSimulationGroup.SortSystems();
    }

    static void AddToWorldAndGroupSystemManaged<T>(T system, World w, ComponentSystemGroup simulationGroup) where T : ComponentSystemBase
    {
        var addedSystem = w.AddSystemManaged(system);
        simulationGroup.AddSystemToUpdateList(addedSystem);
    }
}
