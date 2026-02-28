
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        [SerializeField] private GameContext gameContext;
        [SerializeField] private UpdateHandler updateHandler;
        [SerializeField] private RangeCollider rangeCollider;
        [Space(16)]




        [SerializeField] private Material[] minoMaterials;

        [SerializeField] private Transform[] gridTransforms;
        [SerializeField] private Renderer[] gridRenderers;

        [SerializeField] private Renderer[] holdMinoRenderers;
        [SerializeField] private Renderer[] nextMinoRenderers;
        [Space(16)]




        [SerializeField] private GameObject logoImage;
        [SerializeField] private GameObject resetButton;
        [SerializeField] private TextMeshProUGUI resetButtonTMP;
        [SerializeField] private TextMeshProUGUI nameTMP;
        [Space(16)]
        [SerializeField] private GameObject statUIParent;
        [SerializeField] private TextMeshProUGUI levelTMP, scoreTMP, scoreDeltaTMP;
        [SerializeField] private TextMeshProUGUI sLineTMP, sComboTMP, sTSpinTMP, sBTBTMP, sPerfectTMP;
        [Space(16)]
        [SerializeField] private GameObject techniqueUIParent;
        [SerializeField] private TextMeshProUGUI tLineTMP, tComboTMP, tTSpinTMP, tBTBTMP, tPerfectTMP;
        [Space(16)]
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private Renderer guide;
        [SerializeField] private Material[] guideMaterial;
        [SerializeField] private GameObject titleMinos;
        [SerializeField] private GameObject fake;
        [Space(16)]




        [SerializeField] private GameObject chair;
        [SerializeField] private VRCStation station;
        [SerializeField] private Transform chairUpperLimit;
        [SerializeField] private Transform chairLowerLimit;
    }
}