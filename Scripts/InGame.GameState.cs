
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


    public enum ClearAnimationState : int
    {
        Idle,
        Cleaning,
        Shifting,
        Finishing,
        Completed
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
                    CurrentClearAnimState = ClearAnimationState.Idle;
                }
            }
        }

        private ClearAnimationState _currentClearAnimState = ClearAnimationState.Idle;
        private ClearAnimationState CurrentClearAnimState
        {
            get { return _currentClearAnimState; }
            set
            {
                if (CurrentGameState == GameState.Playing)
                {
                    _currentClearAnimState = value;
                }
                else
                {
                    _currentClearAnimState = ClearAnimationState.Idle;
                }
            }
        }

        private int PlayId { get; set; } = 0; // 各プレイの識別ID
        private string PlayerName { get; set; } = string.Empty;

        private bool IsSitting { get; set; } = false;

        private Vector2Int[] currentMinoPos = new Vector2Int[4];
        private MinoType CurrentMinoType { get; set; } = MinoType.None;
        private int _angle = 0;
        private int Angle { get => _angle; set => _angle = (value % 360 + 360) % 360; } // enum の方がいいかも


        private bool CanHold { get; set; } = true;




        private void GST_Reset()
        {
            PlayerName = Networking.GetOwner(this.gameObject).displayName;

            for (int i = 0; i < currentMinoPos.Length; i++)
            {
                currentMinoPos[i] = Vector2Int.zero;
            }
            CurrentMinoType = MinoType.None;
            Angle = 0;

            CanHold = true;
        }


        private bool IsPlayingGame()
        {
            if (CurrentGameState == GameState.Playing)
            {
                return true;
            }
            return false;
        }


        private bool GST_IsAnimationgClear()
        {
            if (CurrentClearAnimState == ClearAnimationState.Idle)
            {
                return false;
            }
            return true;
        }
    }
}