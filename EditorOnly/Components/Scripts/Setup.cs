using System.Collections.Generic;
using UdonSharp;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    public class Setup : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public static class FieldNames
        {
            public const string NumberOfMain       = nameof(_numberOfMain);
            public const string NumberOfStatsBoard = nameof(_numberOfStatsBoard);
            public const string UsePersistence     = nameof(_usePersistence);
            public const string UdonChips          = nameof(_udonChips);
            public const string UdonChipsRate      = nameof(_udonChipsRate);

            public const string Mains            = nameof(_mains);
            public const string StatsBoards      = nameof(_statsBoards);
            public const string UserDataAccessor = nameof(_userDataAccessor);

            public const string MainPrefab             = nameof(_mainPrefab);
            public const string StatsBoardPrefab       = nameof(_statsBoardPrefab);
            public const string UserDataAccessorPrefab = nameof(_userDataAccessorPrefab);

            public const string _NumberOfMain       = nameof(__numberOfMain);
            public const string _NumberOfStatsBoard = nameof(__numberOfStatsBoard);
            public const string _UsePersistence     = nameof(__usePersistence);
            public const string _UdonChips          = nameof(__udonChips);
            public const string _UdonChipsRate      = nameof(__udonChipsRate);
        }




        [SerializeField] private int _numberOfMain = 1;
        [SerializeField] private int _numberOfStatsBoard = 1;
        [SerializeField] private bool _usePersistence = true;
        [SerializeField] private UdonSharpBehaviour _udonChips;
        [SerializeField] private float _udonChipsRate = 0.1f;

        [SerializeField] private List<GameObject> _mains;
        [SerializeField] private List<GameObject> _statsBoards;
        [SerializeField] private GameObject _userDataAccessor;

        [SerializeField] private GameObject _mainPrefab;
        [SerializeField] private GameObject _statsBoardPrefab;
        [SerializeField] private GameObject _userDataAccessorPrefab;

        [SerializeField] private int __numberOfMain = 1;
        [SerializeField] private int __numberOfStatsBoard = 1;
        [SerializeField] private bool __usePersistence = true;
        [SerializeField] private UdonSharpBehaviour __udonChips;
        [SerializeField] private float __udonChipsRate = 0.1f;

        public int NumberOfMain => _numberOfMain;
        public int NumberOfStatsBoard => _numberOfStatsBoard;
        public bool UsePersistence => _usePersistence;
        public UdonSharpBehaviour UdonChips => _udonChips;
        public float UdonChipsRate => _udonChipsRate;

        public List<GameObject> Mains => _mains;
        public List<GameObject> StatsBoards => _statsBoards;
        public GameObject UserDataAccessor => _userDataAccessor;

        public GameObject MainPrefab => _mainPrefab;
        public GameObject StatsBoardPrefab => _statsBoardPrefab;
        public GameObject UserDataAccessorPrefab => _userDataAccessorPrefab;




        //private void OnEnable()
        //{
        // FindObjectを走らせるのは？
        // Manager運用になったからいらないかも（ヒエラルキーにUDON TETが取り残されるケースが稀）
        //}
    }
}

