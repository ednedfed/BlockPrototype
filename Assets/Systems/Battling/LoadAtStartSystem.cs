using Unity.Entities;

[DisableAutoCreation]
partial class LoadAtStartSystem : SystemBase
{
    BlockFactory _blockFactory;
    bool _isLoaded;

    public LoadAtStartSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;
        _isLoaded = false;
    }

    protected override void OnUpdate() 
    {
        if (_isLoaded == false)
        {
            _isLoaded = true;

            LoadAndSaveUtility.LoadGame(_blockFactory);
        }
    }
}
