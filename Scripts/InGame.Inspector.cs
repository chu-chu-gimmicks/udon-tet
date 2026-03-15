
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
        [SerializeField] private TMPro.TextMeshProUGUI playerNameText;
        [SerializeField] private GameObject yourHighScoreUIParent;
        [SerializeField] private TMPro.TextMeshProUGUI yourHighScoreValueText;
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
        [Space(16)]
        [SerializeField] private UnityEngine.UI.Image guideImage;
        [SerializeField] private Sprite[] guideSprite;
        [SerializeField] private GameObject titleMinos;
        [SerializeField] private GameObject fake;
        [Space(32)]




        [SerializeField] private VRCStation station;
        [SerializeField] private Transform stationLowerLimit;
        [SerializeField] private Transform stationUpperLimit;
    }
}