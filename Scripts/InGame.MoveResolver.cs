
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
        private const float _MR_AUTOREPEAT_INTERVAL = 0.1f;
        private const float _MR_AUTOREPEAT_THRESHOLD = 0.2f;

        private float _MR_timer = 0.0f;
        private AxisState _MR_lastInput = AxisState.Neutral;
        private _AutoRepeat _MR_ARState = _AutoRepeat.Idle;

        private Vector2Int[] _MR_minoBuffer = new Vector2Int[4];




        private void MR_Reset()
        {
            _MR_timer = 0.0f;
            _MR_lastInput = AxisState.Neutral;
            _MR_ARState = _AutoRepeat.Idle;
        }


        private void MR_ResolveMove(Vector2Int[] minoPos, out bool hasAppliedMove)
        {
            hasAppliedMove = false;

            if (!_MR_NeedsMove(out _MoveDir dir)) { return; }
            if (!GS_CanReflectInput()) { return; }

            CopyMino(minoPos, _MR_minoBuffer);
            bool success = _MR_TryMove(_MR_minoBuffer, dir);
            if (success)
            {
                hasAppliedMove = true;
                CopyMino(_MR_minoBuffer, minoPos);
            }
        }


        private bool _MR_NeedsMove(out _MoveDir dir)
        {
            dir = _MoveDir.None;

            _MR_timer += Time.deltaTime;

            AxisState inputState = LHInputState;
            switch (inputState)
            {
                case AxisState.Negative: dir = _MoveDir.Left;  break;
                case AxisState.Positive: dir = _MoveDir.Right; break;
            }

            bool wantsToMove = false;

            if (inputState == AxisState.Neutral)
            {
                _MR_timer = 0.0f;
                _MR_lastInput = inputState;
                _MR_ARState = _AutoRepeat.Idle;
                return false;
            }

            if (_MR_lastInput != inputState)
            {
                _MR_ARState = _AutoRepeat.Idle;
            }
            _MR_lastInput = inputState;

            // U# では Enum の Switch 文は使えないぽい！
            if (_MR_ARState == _AutoRepeat.Idle)
            {
                _MR_ARState = _AutoRepeat.Waiting;
                wantsToMove = true;
            }
            else if (_MR_ARState == _AutoRepeat.Waiting)
            {
                if (_MR_timer >= _MR_AUTOREPEAT_THRESHOLD)
                {
                    _MR_ARState = _AutoRepeat.Repeating;
                    wantsToMove = true;
                }
            }
            else if (_MR_ARState == _AutoRepeat.Repeating)
            {
                if (_MR_timer >= _MR_AUTOREPEAT_INTERVAL)
                {
                    wantsToMove = true;
                }
            }

            if (wantsToMove)
            {
                _MR_timer = 0.0f;
                return true;
            }

            return false;
        }


        private bool _MR_TryMove(Vector2Int[] minoPos, _MoveDir dir)
        {
            Debug.Log(dir);

            for (int i = 0; i < _MR_minoBuffer.Length; i++)
            {
                _MR_minoBuffer[i] = minoPos[i] + new Vector2Int((int)dir, 0);
            }

            if (G_IsMinoSafe(_MR_minoBuffer))
            {
                CopyMino(_MR_minoBuffer, minoPos);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}