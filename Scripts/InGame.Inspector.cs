
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        [SerializeField] private Adapter adapter;
        [SerializeField] private UpdateHandler updateHandler;
        [SerializeField] private RangeCollider rangeCollider;
        [SerializeField] private UserDataAccessor userDataAccessor;
        [Space(32)]




        [SerializeField] private Material[] minoMaterials;

        [SerializeField] private Transform[] gridTransforms;
        [SerializeField] private Renderer[] gridRenderers;

        [SerializeField] private Renderer[] holdMinoRenderers;
        [SerializeField] private Renderer[] nextMinoRenderers;
        [Space(32)]




        [SerializeField] private GameObject logoImage;
        [SerializeField] private TMPro.TextMeshProUGUI playerNameLabel;
        [SerializeField] private GameObject yourHighScoreUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI yourHighScoreLabel;
        [Space(16)]
        [SerializeField] private GameObject statsUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI levelLabel, scoreLabel, scoreDeltaLabel;
        [SerializeField] private TMPro.TextMeshProUGUI sLineLabel, sComboLabel, sTSpinLabel, sBTBLabel, sPerfectLabel;
        [Space(16)]
        [SerializeField] private GameObject pupupUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI pLineLabel, pComboLabel, pTSpinLabel, pBTBLabel, pPerfectLabel;
        [Space(16)]
        [SerializeField] private GameObject gameOverUIParent;
        [Space(16)]
        [SerializeField] private GameObject resetButton;
        [SerializeField] private TMPro.TextMeshProUGUI resetButtonLabel;
        [Space(16)]
        [SerializeField] private Renderer guide;
        [SerializeField] private Material[] guideMaterial;
        [SerializeField] private GameObject titleMinos;
        [SerializeField] private GameObject fake;
        [Space(32)]




        [SerializeField] private VRCStation station;
        [SerializeField] private Transform viewHeight;
        [SerializeField] private Transform stationLowerLimit;
        [SerializeField] private Transform stationUpperLimit;
    }
}