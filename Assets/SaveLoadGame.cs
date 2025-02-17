using System.IO;
using UnityEngine;

public class SaveLoadGame : MonoBehaviour
{
    private const string FILENAME = "/save.bin";

    static string location;

    uint saveVersion = 0;

    public SaveData saveData;
    public BlockFactory blockFactory;

    private void Awake()
    {
        location = Application.persistentDataPath + FILENAME;
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
        {
            using (var file = new FileStream(location, FileMode.Create))
            {
                using (BinaryWriter sw = new BinaryWriter(file))
                {
                    sw.Write(saveVersion);//version

                    foreach (var cube in saveData.placedCubes)
                    {
                        sw.Write(0);//cube type

                        sw.Write(cube.Value.transform.position.x);
                        sw.Write(cube.Value.transform.position.y);
                        sw.Write(cube.Value.transform.position.z);

                        sw.Write(cube.Value.transform.rotation.x);
                        sw.Write(cube.Value.transform.rotation.y);
                        sw.Write(cube.Value.transform.rotation.z);
                        sw.Write(cube.Value.transform.rotation.w);
                    }
                }
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            saveData.Clear();

            if (File.Exists(location) == false)
            {
                UnityEngine.Debug.Log($"{location} doesn't exist");

                return;
            }

            using (var file = File.OpenRead(location))
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

                        blockFactory.InstantiateBlock(blockType, pos, rot);
                    }
                }
            }
        }
    }
}
