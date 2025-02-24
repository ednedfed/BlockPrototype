using System.IO;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class SaveLoadGameSystem : SystemBase
{
    uint _saveVersion = 0;

    SaveData _saveData;
    BlockFactory _blockFactory;

    public SaveLoadGameSystem(uint saveVersion, SaveData saveData, BlockFactory blockFactory)
    {
        _saveVersion = saveVersion;
        _saveData = saveData;
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
        {
            using (var file = new FileStream(BlockGameConstants.SaveGame.SaveLocation, FileMode.Create))
            {
                using (BinaryWriter sw = new BinaryWriter(file))
                {
                    sw.Write(_saveVersion);//version

                    foreach (var cube in _saveData.placedCubes)
                    {
                        var placedBlock = cube.Value;
                        var blockTransform = placedBlock.gameObject.transform;

                        sw.Write(placedBlock.blockType);

                        sw.Write(blockTransform.position.x);
                        sw.Write(blockTransform.position.y);
                        sw.Write(blockTransform.position.z);

                        sw.Write(blockTransform.rotation.x);
                        sw.Write(blockTransform.rotation.y);
                        sw.Write(blockTransform.rotation.z);
                        sw.Write(blockTransform.rotation.w);
                    }
                }
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            _blockFactory.ResetIdGen();
            _saveData.Clear();

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
