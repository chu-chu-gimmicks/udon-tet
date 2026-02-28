
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    // 列挙型はクラスの外で宣言
    public enum GameState : int
    {
        Title = 0,
        Playing = 1,
        GameOver = 2
    }


    public enum ClearState : int
    {
        Idle,
        Start,
        Cleaning,
        ShiftDown,
        Finish
    }


    public partial class InGame
    {
        private GameState _currentGameState = GameState.Title;
        public GameState CurrentGameState
        {
            get { return _currentGameState; }
            set
            {
                _currentGameState = value;
                if (_currentGameState != GameState.Playing)
                {
                    CurrentClearState = ClearState.Idle;
                }
            }
        }

        private ClearState _currentClearState = ClearState.Idle;
        private ClearState CurrentClearState
        {
            get { return _currentClearState; }
            set
            {
                if (CurrentGameState == GameState.Playing)
                {
                    _currentClearState = value;
                }
                else
                {
                    _currentClearState = ClearState.Idle;
                }
            }
        }

        private int PlayId { get; set; } = 0; // 各プレイの識別ID
        private string PlayerName { get; set; } = string.Empty;

        private Vector2Int[] currentMinoPos = new Vector2Int[4];
        private MinoType CurrentMinoType { get; set; } = MinoType.None;
        private int _angle = 0;
        private int Angle { get => _angle; set => _angle = (value % 360 + 360) % 360; } // enum の方がいいかも


        private bool CanHold { get; set; } = true;




        private void GS_Reset()
        {
            // PlayId = 0; これはやっちゃダメ
            PlayerName = Networking.GetOwner(this.gameObject).displayName;

            for (int i = 0; i < currentMinoPos.Length; i++)
            {
                currentMinoPos[i] = Vector2Int.zero;
            }
            CurrentMinoType = MinoType.None;
            Angle = 0;

            CanHold = true;
        }


        private bool GS_CanReflectInput()
        {
            if (CurrentGameState == GameState.Playing && CurrentClearState == ClearState.Idle && Networking.IsOwner(this.gameObject))
            {
                return true;
            }
            return false;
        }


        private bool GS_CanReflectSoftDrop()
        {
            if (CurrentGameState == GameState.Playing && Networking.IsOwner(this.gameObject))
            {
                return true;
            }
            return false;
        }
    }
}