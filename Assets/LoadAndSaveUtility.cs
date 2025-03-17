using System.IO;
using UnityEngine;

public static class LoadAndSaveUtility
{
    public static void LoadGame(BlockFactory blockFactory, int machineId)
    {
        blockFactory.Clear(machineId);

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

                    blockFactory.AddBlock(blockType, pos, rot, machineId);
                }
            }
        }
    }

    public static void SaveGame(uint saveVersion, PlacedBlockContainer placedBlockContainer, int machineId)
    {
        using (var file = new FileStream(BlockGameConstants.SaveGame.SaveLocation, FileMode.Create))
        {
            using (BinaryWriter sw = new BinaryWriter(file))
            {
                sw.Write(saveVersion);//version

                //todo: make entities?
                foreach (var placedBlock in placedBlockContainer.GetValues(machineId))
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
}
