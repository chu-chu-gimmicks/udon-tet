
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class StatsBoard : UdonSharpBehaviour
    {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public static class FieldNames
        {
            public const string PValueText= nameof(_pValueText);
            public const string LNameTexts= nameof(_lNameTexts);
            public const string LValueText= nameof(_lValueText);
            public const string RNameTexts= nameof(_rNameTexts);
            public const string RValueText= nameof(_rValueText);
        }
#endif




        [SerializeField] private TMPro.TextMeshProUGUI   _pValueText;
        [SerializeField] private TMPro.TextMeshProUGUI[] _lNameTexts;
        [SerializeField] private TMPro.TextMeshProUGUI   _lValueText;
        [SerializeField] private TMPro.TextMeshProUGUI[] _rNameTexts;
        [SerializeField] private TMPro.TextMeshProUGUI   _rValueText;




        public void DisplayPersonalData(uint[] values)
        {
            string value = string.Empty;
            value += $"{values[0]}\n";
            value += values[1] >= 30 ? "UDON" : $"{values[1]}\n";
            value += $"{values[2]}";
            _pValueText.text = value;
        }


        public void DisplayLeaderboard(string[] names, uint[] values)
        {
            _lNameTexts[0].text = $"<color=#FFFF00>{names[0]}</color>";
            _lNameTexts[1].text = $"<color=#BFBFBF>{names[1]}</color>";
            _lNameTexts[2].text = $"<color=#DF5F47>{names[2]}</color>";
            _lNameTexts[3].text = $"{names[3]}";
            _lNameTexts[4].text = $"{names[4]}";
            _lNameTexts[5].text = $"{names[5]}";
            _lNameTexts[6].text = $"{names[6]}";
            _lNameTexts[7].text = $"{names[7]}";

            string value = string.Empty;
            value += $"<color=#FFFF00>{values[0]}</color>\n";
            value += $"<color=#BFBFBF>{values[1]}</color>\n";
            value += $"<color=#DF5F47>{values[2]}</color>\n";
            value += $"{values[3]}\n";
            value += $"{values[4]}\n";
            value += $"{values[5]}\n";
            value += $"{values[6]}\n";
            value += $"{values[7]}";
            _lValueText.text = value;
        }


        public void DisplayRecords(string[] names, ushort[] values)
        {
            for (int i = 0; i < _rNameTexts.Length; i++)
            {
                _rNameTexts[i].text = $"{names[i]}";
            }

            string value = string.Empty;
            value += $"{values[0]}\n";
            value += $"{Mathf.Max(values[1] - 1, 0)}\n";
            value += $"{values[2]}\n";
            value += $"{Mathf.Max(values[3] - 1, 0)}\n";
            value += $"{values[4]}";
            _rValueText.text = value;
        }
    }
}