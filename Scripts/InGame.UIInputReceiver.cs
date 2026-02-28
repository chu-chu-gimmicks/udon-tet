
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
            if (CurrentGameState == GameState.Playing) { return; }

            GL_UpdateGameState();

            if (CurrentGameState == GameState.Playing)
            {
                CM_EnterChair();
            }
        }
    }
}
