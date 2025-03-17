using Unity.Entities;

[DisableAutoCreation]
partial class LoadAtStartSystem : SystemBase
{
    BlockFactory _blockFactory;
    SpawnPointComponent[] _spawnPoints;

    public LoadAtStartSystem(BlockFactory blockFactory, SpawnPointComponent[] spawnPoints)
    {
        _blockFactory = blockFactory;
        _spawnPoints = spawnPoints;
    }

    protected override void OnUpdate() 
    {
        for (int i = 0; i < _spawnPoints.Length; ++i)
        {
            if (_spawnPoints[i].isLoaded == false)
            {
                LoadAndSaveUtility.LoadGame(_blockFactory, i);

                _spawnPoints[i].isLoaded = true;
            }
        }
    }
}
