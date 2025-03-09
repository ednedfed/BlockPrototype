using System.IO;
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
            using (var file = new FileStream(BlockGameConstants.SaveGame.SaveLocation, FileMode.Create))
            {
                using (BinaryWriter sw = new BinaryWriter(file))
                {
                    sw.Write(_saveVersion);//version

                    //todo: make entities?
                    foreach (var placedBlock in _placedBlockContainer.GetVales())
                    {
                        sw.Write(placedBlock.blockType);

                        sw.Write(placedBlock.position.x);
                        sw.Write(placedBlock.position.y);
                        sw.Write(placedBlock.position.z);

                        sw.Write(placedBlock.rotation.x);
                        sw.Write(placedBlock.rotation.y);
                        sw.Write(placedBlock.rotation.z);
                        sw.Write(placedBlock.rotation.w);
                    }
                }
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            _blockFactory.ResetIdGen();
            _placedBlockContainer.Clear();

            if (File.Exists(BlockGameConstants.SaveGame.SaveLocation) == false)
            {
                UnityEngine.Debug.Log($"{BlockGameConstants.SaveGame.SaveLocation} doesn't exist");

                return;
            }

            using (var file = File.OpenRead(BlockGameConstants.SaveGame.SaveLocation))
            {
                using (BinaryReader sr = new BinaryReader(file))
                {
                    var version = sr.ReadUInt32();//version

                    //assume it's all blocks, for now should be
                    while (file.Position < file.Length)
                    {
                        var blockType = sr.ReadUInt32();

                        Vector3 pos;
                        Quaternion rot;

                        pos.x = sr.ReadSingle();
                        pos.y = sr.ReadSingle();
                        pos.z = sr.ReadSingle();

                        rot.x = sr.ReadSingle();
                        rot.y = sr.ReadSingle();
                        rot.z = sr.ReadSingle();
                        rot.w = sr.ReadSingle();

                        _blockFactory.InstantiateBlock(blockType, pos, rot);
                    }
                }
            }
        }
    }
}
