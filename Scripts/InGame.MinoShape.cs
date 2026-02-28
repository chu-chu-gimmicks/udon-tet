
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum MinoType : int
    {
        T = 0,
        I = 1,
        O = 2,
        J = 3,
        L = 4,
        S = 5,
        Z = 6,
        None = 7
    }


    public partial class InGame
    {
        private readonly Vector2Int[][] _MS_minoShapes =
        {
            new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) },  // T
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },  // I
            new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) },   // O
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) }, // J
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },  // L
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },  // S
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0) }   // Z
        };


        private void MS_GetMino(Vector2Int[] minoPos, MinoType minoType)
        {
            CopyMino(_MS_minoShapes[(int)minoType], minoPos);
        }
    }
}