
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum AxisState
    {
        Neutral,
        Negative,
        Positive
    }

    public enum ButtonState
    {
        Released,
        Pressed
    }


    public partial class InGame
    {
        private const float _IR_STICK_THRESHOLD = 0.75f;

        private AxisState LHInputState { get; set; } = AxisState.Neutral;
        private AxisState LVInputState { get; set; } = AxisState.Neutral;
        // private AxisState RHInputState { get; set; } = AxisState.Neutral;
        private AxisState RVInputState { get; set; } = AxisState.Neutral;

        private AxisState UseInputState  { get; set; } = AxisState.Neutral;
        private AxisState GrabInputState { get; set; } = AxisState.Neutral;

        private ButtonState JumpInputState { get; set; } = ButtonState.Released;




        private void IR_Reset()
        {
            LHInputState = AxisState.Neutral;
            LVInputState = AxisState.Neutral;
            // RHInputState = AxisState.Neutral;
            RVInputState = AxisState.Neutral;

            UseInputState  = AxisState.Neutral;
            GrabInputState = AxisState.Neutral;

            JumpInputState = ButtonState.Released;

            CM_IsSitting = false;
            chair.SetActive(false);
        }


        private void IR_Update()
        {
            _IR_ReceiveInputByDesktop();
        }


        private void _IR_ReceiveInputByDesktop()
        {
            if (!_IR_CanReceiveInput(isInVR: false)) { return; }


            LHInputState = AxisState.Neutral;
            LVInputState = AxisState.Neutral;
            // RHInputState = AxisState.Neutral;
            RVInputState = AxisState.Neutral;

            UseInputState  = AxisState.Neutral;
            GrabInputState = AxisState.Neutral;

            JumpInputState = ButtonState.Released;

            // 左コントローラーの水平入力に対応
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                LHInputState = AxisState.Negative;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                LHInputState = AxisState.Positive;
            }

            // 左コントローラーの垂直入力に対応
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                LVInputState = AxisState.Positive;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                LVInputState = AxisState.Negative;
            }

            // 右コントローラーの垂直入力に対応
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(wheelAxis) >= Mathf.Epsilon)
            {
                RVInputState = wheelAxis < 0
                    ? AxisState.Negative
                    : AxisState.Positive;
            }

            // Use 入力に対応
            if (Input.GetKey(KeyCode.Q))
            {
                UseInputState = AxisState.Negative;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                UseInputState = AxisState.Positive;
            }

            // Grab 入力に対応
            if (Input.GetKey(KeyCode.Space))
            {
                GrabInputState = AxisState.Positive;
            }

            // Jump 入力に対応
            if (Input.GetKey(KeyCode.LeftShift))
            {
                JumpInputState = ButtonState.Pressed;
            }

            // デバッグ用
            if (Input.GetKeyDown(KeyCode.P))
            {
                GL_DebugLevel();
            }
        }


        // 左スティックの水平方向の入力
        public override void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            LHInputState = _IR_GetAxisState(value);
        }


        // 左スティックの垂直方向の入力
        public override void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            LVInputState = _IR_GetAxisState(value);
        }


        // 右スティックの水平方向の入力
        // public override void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
        // {
        //     if (!_IR_CanReceiveInput(isInVR: true)) { return; }

        //     RHInputState = _IR_GetAxisState(value);
        // }


        // 右スティックの垂直方向の入力
        public override void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            RVInputState = _IR_GetAxisState(value);
        }


        // Use 入力
        public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            UseInputState = _IR_GetAxisState(value, args);
        }


        // Grab 入力
        public override void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            GrabInputState = _IR_GetAxisState(value, args);
        }


        // Jump 入力
        public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_IR_CanReceiveInput(isInVR: true)) { return; }

            JumpInputState = GetButtonState(value);
        }


        private AxisState _IR_GetAxisState(float value)
        {
            if (value <= -_IR_STICK_THRESHOLD) return AxisState.Negative;
            if (value >= _IR_STICK_THRESHOLD)  return AxisState.Positive;
            return AxisState.Neutral;
        }


        private AxisState _IR_GetAxisState(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!value) return AxisState.Neutral;

            return args.handType == VRC.Udon.Common.HandType.LEFT
                ? AxisState.Negative
                : AxisState.Positive;
        }


        private ButtonState GetButtonState(bool value)
        {
            return value ? ButtonState.Pressed : ButtonState.Released;
        }


        private bool _IR_CanReceiveInput(bool isInVR)
        {
            return
                CM_IsSitting
                && Networking.LocalPlayer.IsUserInVR() == isInVR
                && (!Utilities.IsValid(VRC.SDK3.Rendering.VRCCameraSettings.PhotoCamera) || !VRC.SDK3.Rendering.VRCCameraSettings.PhotoCamera.Active);
        }
    }
}
