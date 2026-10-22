
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public static class FieldNames
        {
            public const string UdonChips     = nameof(_udonChips);
            public const string UdonChipsRate = nameof(_udonChipsRate);

            public const string UpdateHandler = nameof(_updateHandler);

            public const string MinoMaterials  = nameof(_minoMaterials);
            public const string GridTransforms = nameof(_gridTransforms);
            public const string GridRenderers  = nameof(_gridRenderers);
            public const string HoldRenderers  = nameof(_holdRenderers);
            public const string NextRenderers  = nameof(_nextRenderers);

            public const string PlayerNameText = nameof(_playerNameText);
            public const string TitleUI        = nameof(_titleUI);
            public const string IngameUI       = nameof(_ingameUI);
            public const string StatsValueText = nameof(_statsValueText);
            public const string PopupText      = nameof(_popupText);
            public const string GameOverUI     = nameof(_gameOverUI);
            public const string Button         = nameof(_button);
            public const string ButtonText     = nameof(_buttonText);
            public const string Guide          = nameof(_guide);
            public const string GuideBindings  = nameof(_guideBindings);
            public const string TitleMino      = nameof(_titleMino);

            public const string Station   = nameof(_station);
            public const string MaxHeight = nameof(_maxHeight);
            public const string MinHeight = nameof(_minHeight);

            public const string RangeCollider = nameof(_rangeCollider);
        }
#endif




        [SerializeField] private UdonSharpBehaviour _udonChips;
        [SerializeField] private float _udonChipsRate = 0.1f;

        [SerializeField] private UpdateHandler _updateHandler;

        [SerializeField] private Material[] _minoMaterials;
        [SerializeField] private Transform[] _gridTransforms;
        [SerializeField] private Renderer[] _gridRenderers;
        [SerializeField] private Renderer[] _holdRenderers;
        [SerializeField] private Renderer[] _nextRenderers;

        [SerializeField] private TMPro.TextMeshProUGUI _playerNameText;
        [SerializeField] private GameObject _titleUI;
        [SerializeField] private GameObject _ingameUI;
        [SerializeField] private TMPro.TextMeshProUGUI _statsValueText;
        [SerializeField] private TMPro.TextMeshProUGUI _popupText;
        [SerializeField] private GameObject _gameOverUI;
        [SerializeField] private GameObject _button;
        [SerializeField] private TMPro.TextMeshProUGUI _buttonText;
        [SerializeField] private GameObject _guide;
        [SerializeField] private GameObject[] _guideBindings;
        [SerializeField] private GameObject _titleMino;

        [SerializeField] private VRCStation _station;
        [SerializeField] private Transform _maxHeight;
        [SerializeField] private Transform _minHeight;

        [SerializeField] private Transform _rangeCollider;
    }
}