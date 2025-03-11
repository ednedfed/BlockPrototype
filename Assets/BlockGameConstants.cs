using Unity.Mathematics;
using UnityEngine;

public abstract class BlockGameConstants
{
    public static class Drone
    {
        public const float MoveSpeed = 500f;
    }

    public static class Camera
    {
        public const float RotateSpeed = 0.3f;
        public readonly static float3 MachineOffset = new float3(0f, 2f, -8f);
    }

    public static class GhostBlock
    {
        public const int BlockTypeCount = 10;

        public const float StartRaycastDistance = 0.5f;
        public const float RaycastDistance = 100f;

        public const int NumDirections = 4;
        public const float DegreesPerTurn = 360f / NumDirections;

        public readonly static Color InvalidGhostColor = new Color { r = 1f, a = 0.25f };
        public readonly static Color InvalidCursorColor = new Color { r = 1f, a = 0.75f };

        public readonly static Color ValidGhostColor = new Color { r = 1f, g = 1f, b = 1f, a = 0.25f };
        public readonly static Color ValidCursorColor = new Color { g = 1f, a = 0.75f };
    }

    public static class BlockProperties
    {
        public const float CubeSize = 1f;
        public const float HalfCubeSize = CubeSize / 2f;

        public readonly static float3 CubeHalfExtent = new float3(0.49f);
        public readonly static float3 CubeCentre = new float3(0f, HalfCubeSize, 0f);
    }

    public static class GameLayers
    { 
        public const int BlockLayer = 3;
        public const int GhostLayer = 6;

        public const int GhostLayerMask = 1 << GhostLayer;
        public const int InverseGhostLayerMask = ~GhostLayerMask;
    }

    public static class TitleScreen
    {
        public const float FlashSpeed = 3f;
        public const float AlphaModifier = 0.7f;
    }

    public static class SaveGame
    {
        const string FileName = "/save.bin";
        public readonly static string SaveLocation = Application.persistentDataPath + FileName;
    }
}
