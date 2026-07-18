
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        #region UdonSharpBehaviour
        [Header("UdonSharpBehaviour")]
        [SerializeField] private UDONTETAuthoring authoring;
        [SerializeField] private UpdateHandler updateHandler;
        #endregion




        #region Field
        [Space(32)]
        [Header("Field")]
        [SerializeField] private Material[] minoMaterials;

        [SerializeField] private Transform[] gridTransforms;
        [SerializeField] private Renderer[] gridRenderers;

        [SerializeField] private Renderer[] holdMinoRenderers;
        [SerializeField] private Renderer[] nextMinoRenderers;
        #endregion




        #region UI
        [Space(32)]
        [Header("UI")]
        [SerializeField] private GameObject logoImage;
        [SerializeField] private TMPro.TextMeshProUGUI playerNameText;
        [Space(16)]
        [SerializeField] private GameObject statsUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI levelValueText, scoreValueText, scoreDeltaValueText;
        [SerializeField] private TMPro.TextMeshProUGUI sLineValueText, sComboValueText, sTSpinValueText, sBTBValueText, sPerfectValueText;
        [Space(16)]
        [SerializeField] private GameObject pupupUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI pLineValueText, pComboValueText, pTSpinValueText, pBTBValueText, pPerfectValueText;
        [Space(16)]
        [SerializeField] private GameObject gameOverUIParent;
        [Space(16)]
        [SerializeField] private GameObject resetButton;
        [SerializeField] private TMPro.TextMeshProUGUI resetButtonText;
        #endregion



        #region Player Position
        [Space(32)]
        [Header("Player Position")]
        [SerializeField] private VRCStation station;
        [SerializeField] private Transform stationLowerLimit;
        [SerializeField] private Transform stationUpperLimit;
        #endregion




        #region Sync
        [Space(32)]
        [Header("Sync")]
        [SerializeField] private Transform rangeCollider;
        #endregion




        #region Animation
        [Space(32)]
        [Header("Animation")]
        [SerializeField] private Animator gridAnimator;
        #endregion




        #region Other Design
        [Space(32)]
        [Header("Other Design")]
        [SerializeField] private Renderer guide;
        [SerializeField] private Material[] guideMaterials;
        [Space(16)]
        [SerializeField] private GameObject titleMinos;
        #endregion
    }
}