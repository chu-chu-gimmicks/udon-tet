
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        public void UII_OnResetButtonPressed()
        {
            if (CurrGameState == GameState.Playing)
            {
                STM_EnterChair();
                UIM_Resume();
            }
            else
            {
                GLP_UpdateGameState();

                if (CurrGameState == GameState.Playing)
                {
                    STM_EnterChair();
                }
            }
        }
    }
}
