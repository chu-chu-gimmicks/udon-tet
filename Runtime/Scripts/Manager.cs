
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Manager : UdonSharpBehaviour
    {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public static class FieldNames
        {
            public const string Mains= nameof(_mains);
            public const string StatsBoards= nameof(_statsBoards);
            public const string UserDataAccessor= nameof(_userDataAccessor);
        }
#endif




        private const float INTERVAL = 2.0f;
        private const string INITIAL_NAME = "----";

        [SerializeField] private Main[] _mains;
        [SerializeField] private StatsBoard[] _statsBoards;
        [SerializeField] private UserDataAccessor _userDataAccessor;

        [UdonSynced] private uint[] _playIds = new uint[16];

        [UdonSynced] private string[] _lNames = new string[8];
        [UdonSynced] private uint[] _lValues = new uint[8];

        [UdonSynced] private string[] _rNames = new string[5];
        [UdonSynced] private ushort[] _rValues = new ushort[5];

        [UdonSynced] private byte _appliedFlags = 0;

        private uint[] _pValues = new uint[3];

        private bool _isPending = false;
        bool _hasEverBeenInitialized = false;




        private void OnEnable()
        {
            InitializeOnce();

            if (!_isPending)
            {
                CheckResult();
            }
        }


        private void InitializeOnce()
        {
            if (_hasEverBeenInitialized) { return; }
            _hasEverBeenInitialized = true;

            if (Utilities.IsValid(_mains) &&  _mains.Length > 0 && _statsBoards.Length > 0)
            {
                for (int i = 0; i < _mains.Length; i++)
                {
                    if (Utilities.IsValid(_mains[i]))
                    {
                        _mains[i].Subscribe(this);
                    }
                }
            }

            for (int i = 0; i < _pValues.Length; i++)
            {
                _pValues[i] = 0;
            }
            if (Utilities.IsValid(_userDataAccessor))
            {
                _userDataAccessor.Subscribe(this);
                _userDataAccessor.GetData(out _pValues[0], out _pValues[1], out _pValues[2]);
            }

            for (int i = 0; i < _lNames.Length; i++)
            {
                _lNames[i] = INITIAL_NAME;
                _lValues[i] = 0;
            }

            for (int i = 0; i < _rNames.Length; i++)
            {
                _rNames[i] = INITIAL_NAME;
                _rValues[i] = 0;
            }

            UpdateDisplay(true, true, true);
        }


        // 一度だけ呼ばれる
        public void OnUserDataRestored()
        {
            uint score = 0, level = 0, count = 0;
            if (Utilities.IsValid(_userDataAccessor))
            {
                _userDataAccessor.GetData(out score, out level, out count);
            }
            
            bool hasChanged = UpdatePersonalData(score, level, count);
            if (hasChanged)
            {
                UpdateDisplay(true, false, false);
            }
        }


        public void OnGameFinished(uint score, uint level, uint count)
        {
            bool hasChanged = UpdatePersonalData(score, level, count);
            if (hasChanged)
            {
                UpdateDisplay(true, false, false);
            }
        }


        private bool UpdatePersonalData(uint score, uint level, uint count)
        {
            const uint MAX_VALUE = 99_999_999;
            score = score > MAX_VALUE ? MAX_VALUE : score;
            level = level > MAX_VALUE ? MAX_VALUE : level;
            count = count > MAX_VALUE - _pValues[2] ? MAX_VALUE - _pValues[2] : count;

            if (score < _pValues[0] && level < _pValues[1] && count <= 0) { return false; }

            _pValues[0] = score > _pValues[0] ? score : _pValues[0];
            _pValues[1] = level > _pValues[1] ? level : _pValues[1];
            _pValues[2] += count;

            if (Utilities.IsValid(_userDataAccessor))
            {
                _userDataAccessor.SetData(_pValues[0], _pValues[1], _pValues[2]);
            }
            return true;
        }


        public void CheckResult()
        {
            _isPending = false;
            if (!Networking.IsOwner(this.gameObject))                         { return; }
            if (!this.gameObject.activeInHierarchy)                           { return; }
            if (!Utilities.IsValid(_mains) && _mains.Length <= 0)             { return; }
            if (!Utilities.IsValid(_statsBoards) && _statsBoards.Length <= 0) { return; }
            _isPending = true;

            SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);

            byte flags = UpdateInstanceStats(_mains, out bool hasLChanged, out bool hasRChanged);
            if (flags > 0)
            {
                UpdateDisplay(false, hasLChanged, hasRChanged);

                for (int i = 0; i < _mains.Length; i++)
                {
                    if (!Utilities.IsValid(_mains[i])) { continue; }
                    if ((flags & (1 << i)) == 0) { continue; }
                    // StatsBoard に反映されたことを伝える（オーナーのみ）
                    _mains[i].OnStatsBoardUpdated();
                }
                _appliedFlags = flags;
                RequestSerialization();
                Debug.Log($"[<color=#ffbf00>UDON TET/Manager</color>] {nameof(CheckResult)} : _appliedFlags={System.Convert.ToString(_appliedFlags, 2).PadLeft(8, '0')}");
            }
            //else
            //{
            //    if (_appliedFlags > 0)
            //    {
            //        _appliedFlags = flags;
            //        Debug.Log($"[<color=#ffbf00>UDON TET/Manager</color>] {nameof(CheckResult)} : _appliedFlags={_appliedFlags}");
            //        RequestSerialization();
            //    }
            //}
        }


        private byte UpdateInstanceStats(Main[] mains, out bool hasLChanged, out  bool hasRChanged)
        {
            byte flags = 0;

            hasLChanged = false;
            hasRChanged = false;

            for (int i = 0; i < mains.Length; i++)
            {
                if (!Utilities.IsValid(mains[i])) { continue; }
                if (mains[i].CurrGameState != GameState.GameOver) { continue; }

                // Play ID はゲームオーバー時に更新される（結果が確定したことを検知できる）
                uint playId = mains[i].GetPlayID();
                if (playId <= _playIds[i] && playId != 0) { continue; }
                _playIds[i] = playId;

                mains[i].GetGameStats(out string playerName, out uint level, out uint score, out uint count, out ushort line, out ushort comb, out ushort tspn, out ushort btob, out ushort perf);
                hasLChanged |= UpdateLeaderboard(playerName, score);
                hasRChanged |= UpdateRecords(playerName, line, comb, tspn, btob, perf);

                flags |= (byte)(1 << i);
            }
            return flags;
        }


        private bool UpdateLeaderboard(string name, uint score)
        {
            const uint MAX_VALUE = 99_999_999;
            score = score > MAX_VALUE ? MAX_VALUE : score;

            if (score <= _lValues[_lValues.Length - 1]) { return false; }

            int length = _lNames.Length;
            int oldIndex = -1;

            for (int i = 0; i < length; i++)
            {
                if (_lNames[i] == name)
                {
                    oldIndex = i;
                    break;
                }
            }

            // 同名プレイヤーのスコアが既にあるなら
            if (oldIndex >= 0)
            {
                // 自分のスコアが更新されていなければ何もしない
                if (score <= _lValues[oldIndex]) { return false; }

                // 該当プレイヤーのスコアを抜いたランキングを作る
                for (int i = oldIndex; i < length - 1; i++)
                {
                    _lNames[i] = _lNames[i + 1];
                    _lValues[i] = _lValues[i + 1];
                }
                _lNames[length - 1] = string.Empty;
                _lValues[length - 1] = 0;
            }

            // 新しいスコアを入れる
            for (int i = 0; i < length; i++)
            {
                if (score <= _lValues[i]) { continue; }

                for (int j = length - 1; j > i; j--)
                {
                    _lNames[j] = _lNames[j - 1];
                    _lValues[j] = _lValues[j - 1];
                }
                _lNames[i] = name;
                _lValues[i] = score;
                break;
            }

            return true;
        }


        private bool UpdateRecords(string name, ushort line, ushort comb, ushort tspn, ushort btob, ushort perf)
        {
            bool hasChanged = false;
            
            const ushort MAX_VALUE = 9_999;
            line = line > MAX_VALUE ? MAX_VALUE : line;
            comb = comb > MAX_VALUE ? MAX_VALUE : comb;
            tspn = tspn > MAX_VALUE ? MAX_VALUE : tspn;
            btob = btob > MAX_VALUE ? MAX_VALUE : btob;
            perf = perf > MAX_VALUE ? MAX_VALUE : perf;

            if (line > _rValues[0])
            {
                _rNames[0] = name;
                _rValues[0] = line;
                hasChanged = true;
            }
            if (comb > _rValues[1] && comb > 1)
            {
                _rNames[1] = name;
                _rValues[1] = comb;
                hasChanged = true;
            }
            if (tspn > _rValues[2])
            {
                _rNames[2] = name;
                _rValues[2] = tspn;
                hasChanged = true;
            }
            if (btob > _rValues[3] && btob > 1)
            {
                _rNames[3] = name;
                _rValues[3] = btob;
                hasChanged = true;
            }
            if (perf > _rValues[4])
            {
                _rNames[4] = name;
                _rValues[4] = perf;
                hasChanged = true;
            }

            return hasChanged;
        }


        private void UpdateDisplay(bool hasPChanged, bool hasLChanged, bool hasRChanged)
        {
            if (!Utilities.IsValid(_statsBoards) || _statsBoards.Length == 0) { return; }

            for (int i = 0; i < _statsBoards.Length; i++)
            {
                if (!Utilities.IsValid(_statsBoards[i])) { continue; }
                if (hasPChanged)
                {
                    _statsBoards[i].DisplayPersonalData(_pValues);
                }
                if (hasLChanged)
                {
                    _statsBoards[i].DisplayLeaderboard(_lNames, _lValues);
                }
                if (hasRChanged)
                {
                    _statsBoards[i].DisplayRecords(_rNames, _rValues);
                }
            }
        }


        public override void OnDeserialization()
        {
            Debug.Log($"[<color=#ffbf00>UDON TET/Manager</color>] {nameof(OnDeserialization)} : _appliedFlags={System.Convert.ToString(_appliedFlags, 2).PadLeft(8, '0')}");

            UpdateDisplay(false, true, true);

            if (!Utilities.IsValid(_mains) || _mains.Length == 0) { return; }
            for (int i = 0; i < _mains.Length; i++)
            {
                if (!Utilities.IsValid(_mains[i])) { continue; }
                if ((_appliedFlags & (1 << i)) == 0) { continue; }
                // StatsBoard に反映されたことを伝える（オーナー以外）
                _mains[i].OnStatsBoardUpdated();
            }
        }


        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player.isLocal && !_isPending)
            {
                CheckResult();
            }
        }
    }
}
