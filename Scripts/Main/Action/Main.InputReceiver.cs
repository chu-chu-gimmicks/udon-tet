
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private const float _INR_STICK_THRESHOLD = 0.75f;

        private AxisState InputStateLX { get; set; } = AxisState.Neutral;
        private AxisState InputStateLY { get; set; } = AxisState.Neutral;
        private AxisState InputStateRX { get; set; } = AxisState.Neutral;
        private AxisState InputStateRY { get; set; } = AxisState.Neutral;

        // private ButtonState InputStateUseL  { get; set; }  = ButtonState.Released;
        private ButtonState InputStateUseR  { get; set; }  = ButtonState.Released;
        private ButtonState InputStateGrabL { get; set; } = ButtonState.Released;
        private ButtonState InputStateGrabR { get; set; } = ButtonState.Released;
        private ButtonState InputStateJump  { get; set; }  = ButtonState.Released;




        private void INR_Reset()
        {
            InputStateLX = AxisState.Neutral;
            InputStateLY = AxisState.Neutral;
            InputStateRX = AxisState.Neutral;
            InputStateRY = AxisState.Neutral;

            // InputStateUseL  = ButtonState.Released;
            InputStateUseR  = ButtonState.Released;
            InputStateGrabL = ButtonState.Released;
            InputStateGrabR = ButtonState.Released;
            InputStateJump  = ButtonState.Released;
        }


        private void INR_Update()
        {
            _INR_ReceiveInputByDesktop();
        }


        private void _INR_ReceiveInputByDesktop()
        {
            if (!_INR_CanReceiveInput(isInVR: false)) { return; }

            bool negDown = false;
            bool posDown = false;
            bool negHeld = false;
            bool posHeld = false;

            // 左コントローラーの水平入力に対応
            negDown = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            posDown = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
            negHeld = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            posHeld = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            InputStateLX = _INR_GetAxisState(negDown, posDown, negHeld, posHeld, InputStateLX);

            // 左コントローラーの垂直入力に対応
            negDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
            posDown = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            negHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            posHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            InputStateLY = _INR_GetAxisState(negDown, posDown, negHeld, posHeld, InputStateLY);

            // 右コントローラーの水平入力に対応
            negDown = Input.GetKeyDown(KeyCode.Q);
            posDown = Input.GetKeyDown(KeyCode.E);
            negHeld = Input.GetKey(KeyCode.Q);
            posHeld = Input.GetKey(KeyCode.E);
            InputStateRX = _INR_GetAxisState(negDown, posDown, negHeld, posHeld, InputStateRX);

            // 右コントローラーの垂直入力に対応
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(wheelAxis) >= Mathf.Epsilon)
            {
                InputStateRY = wheelAxis < 0
                    ? AxisState.Negative
                    : AxisState.Positive;
            }
            else
            {
                InputStateRY = AxisState.Neutral;
            }

            // Use 入力に対応
            if (Input.GetMouseButton(1))
            {
                InputStateUseR = ButtonState.Pressed;
            }
            else
            {
                InputStateUseR = ButtonState.Released;
            }

            // Grab 入力に対応
            if (Input.GetKey(KeyCode.Space))
            {
                InputStateGrabL = ButtonState.Pressed;
                InputStateGrabR = ButtonState.Pressed;
            }
            else
            {
                InputStateGrabL = ButtonState.Released;
                InputStateGrabR = ButtonState.Released;
            }

            // Jump 入力に対応
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                InputStateJump = ButtonState.Pressed;
            }
            else
            {
                InputStateJump = ButtonState.Released;
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

            InputStateLX = _INR_GetAxisState(value);
        }


        // 左スティックの垂直方向の入力
        public override void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            InputStateLY = _INR_GetAxisState(value);
        }


        // 右スティックの水平方向の入力
        public override void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            InputStateRX = _INR_GetAxisState(value);
        }


        // 右スティックの垂直方向の入力
        public override void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            InputStateRY = _INR_GetAxisState(value);
        }


        // Use 入力
        public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            switch (args.handType)
            {
                case VRC.Udon.Common.HandType.LEFT:
                    // InputStateUseL = _INR_GetButtonState(value);
                    break;
                case VRC.Udon.Common.HandType.RIGHT:
                    InputStateUseR = _INR_GetButtonState(value);
                    break;
            }
        }


        // Grab 入力
        public override void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            switch (args.handType)
            {
                case VRC.Udon.Common.HandType.LEFT:
                    InputStateGrabL = _INR_GetButtonState(value);
                    break;
                case VRC.Udon.Common.HandType.RIGHT:
                    InputStateGrabR = _INR_GetButtonState(value);
                    break;
            }
        }


        // Jump 入力
        public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!_INR_CanReceiveInput(isInVR: true)) { return; }

            InputStateJump = _INR_GetButtonState(value);
        }


        private AxisState _INR_GetAxisState(bool negDown, bool posDown, bool negHeld, bool posHeld, AxisState currState)
        {
            AxisState newState = currState;
            // 新たに押されたかどうか
            if (negDown)
            {
                newState = AxisState.Negative;
            }
            else if (posDown)
            {
                newState = AxisState.Positive;
            }
            // 継続して押されているかどうか
            if (currState == AxisState.Negative && !negHeld)
            {
                newState = posHeld
                    ? AxisState.Positive
                    : AxisState.Neutral;
            }
            else if (currState == AxisState.Positive && !posHeld)
            {
                newState = negHeld
                    ? AxisState.Negative
                    : AxisState.Neutral;
            }
            return newState;
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
