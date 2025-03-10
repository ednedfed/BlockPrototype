using Unity.Entities;

[DisableAutoCreation]
partial class LoadAtStartSystem : SystemBase
{
    BlockFactory _blockFactory;

    public LoadAtStartSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;
    }

    protected override void OnCreate()
    {
        LoadAndSaveUtility.LoadGame(_blockFactory);
    }

    protected override void OnUpdate() { }
}
