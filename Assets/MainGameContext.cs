using UnityEngine;

public partial class MainGameContext : MonoBehaviour
{
    //scene objects, make spawn later
    public GameObject character;
    public GameObject ghost;
    public GameObject cursor;

    //will become systems after we move everything off game object and it still works
    FirstPersonController _firstPersonController;
    DeleteBlock _deleteBlockSystem;
    PlaceBlock _placeBlockSystem;
    GhostBlockType _ghostBlockType;
    GhostPosition _ghostPosition;
    GhostOverlappingSync _ghostOverlappingSync;
    GhostRotation _ghostRotation;
    SaveLoadGame _saveLoadGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //todo: also load static data?
        BlockTypes blockTypes = GameObject.FindFirstObjectByType<BlockTypes>();

        HitObject hitObject = new HitObject();
        GhostBlockData ghostBlockData = new GhostBlockData();
        SaveData saveData = new SaveData();
        BlockFactory blockFactory = new BlockFactory(blockTypes, saveData);

        //separate what will become systems here, once data is split we can work on making real systems
        _firstPersonController = new FirstPersonController();
        _firstPersonController.charTransform = character.transform;
        _firstPersonController.rb = character.GetComponent<Rigidbody>();
        _firstPersonController.Start();

        _deleteBlockSystem = new DeleteBlock(hitObject, blockFactory);
        _placeBlockSystem = new PlaceBlock(ghost, hitObject, ghostBlockData, blockFactory);
        _ghostBlockType = new GhostBlockType(ghost, ghostBlockData, blockTypes);
        _ghostRotation = new GhostRotation(ghostBlockData);
        _ghostPosition = new GhostPosition(character, cursor, ghost, ghostBlockData, hitObject);
        _ghostOverlappingSync = new GhostOverlappingSync(cursor, ghost, hitObject);
        _saveLoadGame = new SaveLoadGame(0, saveData, blockFactory);

        //inject, but later use ECS world so don't write an entire DI system yet
        DependencyInjection.InjectIntoMonobehaviours(blockTypes);
        DependencyInjection.InjectIntoMonobehaviours(hitObject);
        DependencyInjection.InjectIntoMonobehaviours(ghostBlockData);
        DependencyInjection.InjectIntoMonobehaviours(saveData);
        DependencyInjection.InjectIntoMonobehaviours(blockFactory);
    }

    // Update is called once per frame
    void Update()
    {
        _deleteBlockSystem.Update();
        _placeBlockSystem.Update();
        _ghostBlockType.Update();
        _ghostRotation.Update();
        _saveLoadGame.Update();
    }

    void FixedUpdate()
    {
        _firstPersonController.FixedUpdate();
        _ghostPosition.FixedUpdate();
        _ghostOverlappingSync.FixedUpdate();
    }
}
