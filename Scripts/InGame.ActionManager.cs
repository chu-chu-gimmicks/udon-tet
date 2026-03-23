
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
        ChairAdjust = 64,
        Pause       = 128,
        Guide       = 256
    }


    public partial class InGame
    {
        private void ACM_Reset()
        {
        }


        private int ACM_ResolvedActions(Vector2Int[] minoPos)
        {
            int actions = (int)PlayerAction.None;

            if (MVR_ResolveMove(minoPos))     { actions |= (int)PlayerAction.Move; }
            if (SPR_ResolveSpin(minoPos))     { actions |= (int)PlayerAction.Spin; }
            if (SDR_ResolveSoftDrop())        { actions |= (int)PlayerAction.SoftDrop; }
            if (HDR_ResolveHardDrop(minoPos)) { actions |= (int)PlayerAction.HardDrop; }

            if (HLR_ResolveHold(minoPos, out bool isFirstHold))
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

            if (ADR_ResolveAdjustment())  { actions |= (int)PlayerAction.ChairAdjust; }
            if (PAR_ResolvePause())       { actions |= (int)PlayerAction.Pause; }
            if (GDR_ResolveGuide())       { actions |= (int)PlayerAction.Guide; }

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