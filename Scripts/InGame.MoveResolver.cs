
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum _MoveDir
    {
        None = 0,
        Left = -1,
        Right = 1
    }


    public enum _AutoRepeat
    {
        Idle,
        Waiting,
        Repeating
    }


    public partial class InGame
    {
        private const float _MVR_AUTOREPEAT_INTERVAL = 0.1f;
        private const float _MVR_AUTOREPEAT_THRESHOLD = 0.3f;

        private float _MVR_timer = 0.0f;
        private AxisState _MVR_lastInput = AxisState.Neutral;
        private _AutoRepeat _MVR_ARState = _AutoRepeat.Idle;

        private Vector2Int[] _MVR_minoBuffer = new Vector2Int[4];




        private void MVR_Reset()
        {
            _MVR_timer = 0.0f;
            _MVR_lastInput = AxisState.Neutral;
            _MVR_ARState = _AutoRepeat.Idle;
        }


        private bool MVR_ResolveMove(Vector2Int[] minoPos)
        {
            if (!_MVR_NeedsMove(out _MoveDir dir)) { return false; }

            CopyMino(minoPos, _MVR_minoBuffer);
            bool success = _MVR_TryMove(_MVR_minoBuffer, dir);
            if (success)
            {
                CopyMino(_MVR_minoBuffer, minoPos);
                return true;
            }
            return false;
        }


        private bool _MVR_NeedsMove(out _MoveDir dir)
        {
            dir = _MoveDir.None;

            _MVR_timer += Time.deltaTime;

            AxisState inputState = InputStateLX;
            switch (inputState)
            {
                case AxisState.Negative: dir = _MoveDir.Left;  break;
                case AxisState.Positive: dir = _MoveDir.Right; break;
            }

            bool wantsToMove = false;

            if (inputState == AxisState.Neutral)
            {
                _MVR_timer = 0.0f;
                _MVR_lastInput = inputState;
                _MVR_ARState = _AutoRepeat.Idle;
                return false;
            }

            if (_MVR_lastInput != inputState)
            {
                _MVR_ARState = _AutoRepeat.Idle;
            }
            _MVR_lastInput = inputState;

            // U# では Enum の Switch 文は使えないぽい！
            if (_MVR_ARState == _AutoRepeat.Idle)
            {
                _MVR_ARState = _AutoRepeat.Waiting;
                wantsToMove = true;
            }
            else if (_MVR_ARState == _AutoRepeat.Waiting)
            {
                if (_MVR_timer >= _MVR_AUTOREPEAT_THRESHOLD)
                {
                    _MVR_ARState = _AutoRepeat.Repeating;
                    wantsToMove = true;
                }
            }
            else if (_MVR_ARState == _AutoRepeat.Repeating)
            {
                if (_MVR_timer >= _MVR_AUTOREPEAT_INTERVAL)
                {
                    wantsToMove = true;
                }
            }

            if (wantsToMove)
            {
                _MVR_timer = 0.0f;
                return true;
            }

            return false;
        }


        private bool _MVR_TryMove(Vector2Int[] minoPos, _MoveDir dir)
        {
            for (int i = 0; i < _MVR_minoBuffer.Length; i++)
            {
                _MVR_minoBuffer[i] = minoPos[i] + new Vector2Int((int)dir, 0);
            }

            if (GRD_IsMinoSafe(_MVR_minoBuffer))
            {
                CopyMino(_MVR_minoBuffer, minoPos);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}