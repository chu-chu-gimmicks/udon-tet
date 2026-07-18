
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private void ACM_Reset()
        {
            INR_Reset();

            HLR_Reset();

            MVR_Reset();
            RTR_Reset();
            HDR_Reset();

            SDR_Reset();
            ADR_Reset();
            PAR_Reset();
            GDR_Reset();
        }


        private int ACM_ResolvedActions(Vector2Int[] minoPos)
        {
            int actions = (int)PlayerAction.None;

            if (HLR_ResolveHold(minoPos, out bool isFirstHold))
            {
                actions |= isFirstHold ? (int)PlayerAction.FirstHold : (int)PlayerAction.Hold;
            }

            if (MVR_ResolveMove(minoPos))     { actions |= (int)PlayerAction.Move; }
            if (RTR_ResolveRotation(minoPos)) { actions |= (int)PlayerAction.Spin; }
            if (HDR_ResolveHardDrop(minoPos)) { actions |= (int)PlayerAction.HardDrop; }

            if (SDR_ResolveSoftDrop())   { actions |= (int)PlayerAction.SoftDrop; }
            if (ADR_ResolveAdjustment()) { actions |= (int)PlayerAction.ChairAdjust; }
            if (PAR_ResolvePause())      { actions |= (int)PlayerAction.Pause; }
            if (GDR_ResolveGuide())      { actions |= (int)PlayerAction.Guide; }

            // 優先順位が大事
            if ((actions & (int)PlayerAction.FirstHold) != 0 || (actions & (int)PlayerAction.Hold) != 0)
            {
                return actions;
            }

            if ((actions & (int)PlayerAction.HardDrop) == 0)
            {
                if ((actions & (int)PlayerAction.Move) != 0)
                {
                    DRS_TSpin = TSpinState.None;
                    LDC_UpdateByInput(minoPos);
                }

                if ((actions & (int)PlayerAction.Spin) != 0)
                {
                    LDC_UpdateByInput(minoPos);
                }
            }

            return actions;
        }


        private int ACM_ResolveActionsWhileAnimating()
        {
            int actions = (int)PlayerAction.None;
            if (SDR_ResolveSoftDrop())   { actions |= (int)PlayerAction.SoftDrop; }
            if (ADR_ResolveAdjustment()) { actions |= (int)PlayerAction.ChairAdjust; }
            if (PAR_ResolvePause())      { actions |= (int)PlayerAction.Pause; }
            if (GDR_ResolveGuide())      { actions |= (int)PlayerAction.Guide; }
            return actions;
        }
    }
}