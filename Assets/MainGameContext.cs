using Unity.Entities;
using UnityEngine;

public partial class MainGameContext : MonoBehaviour
{
    //scene objects, make spawn later
    public GameObject character;
    public GameObject ghost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //todo: also load static data?
        BlockTypes blockTypes = GameObject.FindFirstObjectByType<BlockTypes>();

        HitObject hitObject = new HitObject();
        GhostBlockData ghostBlockData = new GhostBlockData();
        SaveData saveData = new SaveData();
        BlockFactory blockFactory = new BlockFactory(blockTypes, saveData);

        //todo: totally custom world to exclude unnecessary sytems
        var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
        var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new DeleteBlockSystem(hitObject, blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostBlockTypeSyncSystem(ghost, ghostBlockData, blockTypes), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostRotationUpdateSystem(ghostBlockData), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new PlaceBlockSystem(ghost, hitObject, ghostBlockData, blockFactory), world, simulationGroup);
        AddToWorldAndGroupSystemManaged(new SaveLoadGameSystem(0, saveData, blockFactory), world, simulationGroup);

        var fixedStepSimulationGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

        AddToWorldAndGroupSystemManaged(new FirstPersonControllerSystem(character.GetComponent<Rigidbody>(), character.transform), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostPositionUpdateSystem(character, ghost, ghostBlockData, hitObject), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new GhostOverlappingSyncSystem(ghost, hitObject), world, fixedStepSimulationGroup);
        AddToWorldAndGroupSystemManaged(new ParentCursorToGhostSystem(ghost), world, fixedStepSimulationGroup);

        simulationGroup.SortSystems();
        fixedStepSimulationGroup.SortSystems();
    }

    static void AddToWorldAndGroupSystemManaged<T>(T system, World w, ComponentSystemGroup simulationGroup) where T : ComponentSystemBase
    {
        var addedSystem = w.AddSystemManaged(system);
        simulationGroup.AddSystemToUpdateList(addedSystem);
    }
}
