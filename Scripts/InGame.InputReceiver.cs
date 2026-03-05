
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
        private const float _INR_STICK_THRESHOLD = 0.75f;

        private AxisState LHInputState { get; set; } = AxisState.Neutral;
        private AxisState LVInputState { get; set; } = AxisState.Neutral;
        private AxisState RHInputState { get; set; } = AxisState.Neutral;
        private AxisState RVInputState { get; set; } = AxisState.Neutral;

        //private ButtonState LUseInputState { get; set; }  = ButtonState.Released;
        //private ButtonState RUseInputState { get; set; }  = ButtonState.Released;
        private ButtonState LGrabInputState { get; set; } = ButtonState.Released;
        private ButtonState RGrabInputState { get; set; } = ButtonState.Released;
        private ButtonState JumpInputState { get; set; }  = ButtonState.Released;




        private void INR_Reset()
        {
            LHInputState = AxisState.Neutral;
            LVInputState = AxisState.Neutral;
            RHInputState = AxisState.Neutral;
            RVInputState = AxisState.Neutral;

            //LUseInputState  = ButtonState.Released;
            //RUseInputState  = ButtonState.Released;
            LGrabInputState = ButtonState.Released;
            RGrabInputState = ButtonState.Released;
            JumpInputState  = ButtonState.Released;
        }


        private void INR_Update()
        {
            _INR_ReceiveInputByDesktop();
        }


        private void _INR_ReceiveInputByDesktop()
        {
            if (!_INR_CanReceiveInput(isInVR: false)) { return; }


            LHInputState = AxisState.Neutral;
            LVInputState = AxisState.Neutral;
            RHInputState = AxisState.Neutral;
            RVInputState = AxisState.Neutral;

            //LUseInputState = ButtonState.Released;
            //RUseInputState = ButtonState.Released;
            LGrabInputState = ButtonState.Released;
            RGrabInputState = ButtonState.Released;
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

            // 右コントローラーの水平入力に対応
            if (Input.GetKey(KeyCode.Q))
            {
                RHInputState = AxisState.Negative;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                RHInputState = AxisState.Positive;
            }

            // 右コントローラーの垂直入力に対応
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(wheelAxis) >= Mathf.Epsilon)
            {
                RVInputState = wheelAxis < 0
                    ? AxisState.Negative
                    : AxisState.Positive;
            }

            // Grab 入力に対応
            if (Input.GetKey(KeyCode.Space))
            {
                LGrabInputState = ButtonState.Pressed;
                RGrabInputState = ButtonState.Pressed;
            }

            // Jump 入力に対応
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                JumpInputState = ButtonState.Pressed;
            }

            // デバッグ用
            if (Input.GetKeyDown(KeyCode.P))
            {
                GLP_DebugLevel();
            }
        }


        // 左スティックの水平方向の入力
        public override void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            LHInputState = _INR_GetAxisState(value);
        }


        // 左スティックの垂直方向の入力
        public override void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            LVInputState = _INR_GetAxisState(value);
        }


        // 右スティックの水平方向の入力
        public override void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            RHInputState = _INR_GetAxisState(value);
        }


        // 右スティックの垂直方向の入力
        public override void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            RVInputState = _INR_GetAxisState(value);
        }


        // Use 入力
        //public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        //{
        //    if (!_INR_CanReceiveInput(isInVR: true)) { return; }

        //    switch (args.handType)
        //    {
        //        case VRC.Udon.Common.HandType.LEFT:
        //            LUseInputState = _INR_GetButtonState(value);
        //            break;
        //        case VRC.Udon.Common.HandType.RIGHT:
        //            RUseInputState = _INR_GetButtonState(value);
        //            break;
        //    }
        //}


        // Grab 入力
        public override void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            switch (args.handType)
            {
                case VRC.Udon.Common.HandType.LEFT:
                    LGrabInputState = _INR_GetButtonState(value);
                    break;
                case VRC.Udon.Common.HandType.RIGHT:
                    RGrabInputState = _INR_GetButtonState(value);
                    break;
            }
        }


        // Jump 入力
        public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            JumpInputState = _INR_GetButtonState(value);
        }


        private AxisState _INR_GetAxisState(float value)
        {
            if (value <= -_INR_STICK_THRESHOLD) return AxisState.Negative;
            if (value >= _INR_STICK_THRESHOLD)  return AxisState.Positive;
            return AxisState.Neutral;
        }


        private ButtonState _INR_GetButtonState(bool value)
        {
            return value ? ButtonState.Pressed : ButtonState.Released;
        }


        private bool _INR_CanReceiveInput(bool isInVR)
        {
            return
                STM_IsSitting
                && Networking.LocalPlayer.IsUserInVR() == isInVR
                && (!Utilities.IsValid(VRC.SDK3.Rendering.VRCCameraSettings.PhotoCamera) || !VRC.SDK3.Rendering.VRCCameraSettings.PhotoCamera.Active);
        }
    }
}
