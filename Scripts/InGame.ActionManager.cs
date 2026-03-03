
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum PlayerAction : int
    {
        None        = 0,
        Move        = 1,
        Spin        = 2,
        SoftDrop    = 4,
        HardDrop    = 8,
        FirstHold   = 16,
        Hold        = 32,
        ChairAdjust = 64
    }


    public partial class InGame
    {
        private int AM_ResolvedActions()
        {
            int actions = (int)PlayerAction.None;

            if (MR_ResolveMove(currentMinoPos)) { actions |= (int)PlayerAction.Move; }
            if (SR_ResolveSpin(currentMinoPos)) { actions |= (int)PlayerAction.Spin; }
            if (SDR_ResolveSoftDrop()) { actions |= (int)PlayerAction.SoftDrop; }
            if (HDR_ResolveHardDrop(currentMinoPos)) { actions |= (int)PlayerAction.HardDrop; }

            if (HR_ResolveHold(currentMinoPos, out bool isFirstHold))
            {
                if (isFirstHold)
                {
                    actions |= (int)PlayerAction.FirstHold;
                }
                else
                {
                    actions |= (int)PlayerAction.Hold;
                }
            }

            if (CAR_ResolveChairAdjust()) { actions |= (int)PlayerAction.ChairAdjust; }

            // 優先順位が大事
            if ((actions & (int)PlayerAction.Hold) != 0 || (actions & (int)PlayerAction.FirstHold) != 0)
            {
                GR_HideMino(_GL_minoBuffer);
                PR_ShowHoldMino(HR_HoldMinoType);
                return actions;
            }

            if ((actions & (int)PlayerAction.HardDrop) == 0)
            {
                if ((actions & (int)PlayerAction.Move) != 0)
                {
                    DR_TSpin = TSpinState.None;
                    LC_UpdateByInput(currentMinoPos);
                }

                if ((actions & (int)PlayerAction.Spin) != 0)
                {
                    LC_UpdateByInput(currentMinoPos);
                }
            }

            GR_HideMino(_GL_minoBuffer);
            GR_ShowMino(currentMinoPos, CurrentMinoType);
            CopyMino(currentMinoPos, _GL_minoBuffer);

            return actions;
        }
    }
}