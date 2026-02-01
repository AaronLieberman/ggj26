
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class PieceTooltip : MonoBehaviour
{
    public enum Stat
    {
        Scary,
        Goofy,
        Beauty,
        Anonymity
    }

    [SerializeField] public TextMeshProUGUI NameText;
    [SerializeField] public TextMeshProUGUI TypeText;
    [SerializeField] public IconBar[] IconBars;
    [SerializeField] public GridLayoutGroup IconBarGridLayout;

    Dictionary<Stat, IconBar> _iconBarMap = new Dictionary<Stat, IconBar>();

    MaskPartData _data;
    public MaskPartData Data
    {
        set
        {
            _data = value;

            NameText.text = _data.partName;
            TypeText.text = _data.slot.ToString().ToUpper();
            
            foreach (var kvp in _iconBarMap)
            {
                switch (kvp.Key)
                {
                    case Stat.Scary:
                        kvp.Value.SetCount(Data.scaryStat);
                        break;
                    case Stat.Goofy:
                        kvp.Value.SetCount(Data.goofyStat);
                        break;
                    case Stat.Beauty:
                        kvp.Value.SetCount(Data.beautyStat);
                        break;
                    case Stat.Anonymity:
                        kvp.Value.SetCount(Data.anonymityStat);
                        break;
                }
            }
            //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
        }
        get
        {
            return _data;
        }
    }

    public void Awake()
    {
        foreach (var iconBar in IconBars)
        {
            if (System.Enum.TryParse<Stat>(iconBar.name, out var stat))
            {
                _iconBarMap[stat] = iconBar;
            }
            else
            {
                Debug.LogError($"Unknown IconBar name {iconBar.name} on PieceTooltip");
            }
        }
    }

}