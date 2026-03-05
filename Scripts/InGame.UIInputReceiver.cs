
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        public void UII_OnResetButtonPressed()
        {
            if (CurrentGameState == GameState.Playing)
            {
                STM_EnterChair();
                UIM_Continue();
            }
            else
            {
                GLP_UpdateGameState();

                if (CurrentGameState == GameState.Playing)
                {
                    STM_EnterChair();
                }
            }
        }
    }
}
