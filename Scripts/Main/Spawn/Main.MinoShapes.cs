
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private readonly Vector2Int[][] _MSH_minoShapes =
        {
            new[] { new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0) },  // None
            new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) },  // T
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },  // I
            new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) },   // O
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) }, // J
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },  // L
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },  // S
            new[] { new Vector2Int(0, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0) }   // Z
        };




        private void MSH_GetMino(Vector2Int[] minoPos, MinoType minoType)
        {
            CopyMino(_MSH_minoShapes[(int)minoType], minoPos);
        }
    }
}