using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class SaveLoadGameSystem : SystemBase
{
    uint _saveVersion = 0;

    PlacedBlockContainer _placedBlockContainer;
    BlockFactory _blockFactory;

    public SaveLoadGameSystem(uint saveVersion, PlacedBlockContainer placedBlockContainer, BlockFactory blockFactory)
    {
        _saveVersion = saveVersion;
        _placedBlockContainer = placedBlockContainer;
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        //todo: don't convert this yet because it's debug only, eventually make a save option in hud
        if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
        {
            LoadAndSaveUtility.SaveGame(_saveVersion, _placedBlockContainer, BlockGameConstants.SaveGame.BuildMachineId);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            LoadAndSaveUtility.LoadGame(_blockFactory, BlockGameConstants.SaveGame.BuildMachineId);
        }
    }
}
