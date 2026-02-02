using System.Linq;
using UnityEngine;
using System;

public struct SingleStat
{
    public float Progress;
    public bool InRange;
    public int Score;
}

public struct StatsScore
{
    public SingleStat Scary;
    public SingleStat Goofy;
    public SingleStat Beauty;
    public SingleStat Anonymity;

    public readonly bool IsAcceptable()
    {
        return Scary.InRange && Goofy.InRange && Beauty.InRange && Anonymity.InRange;
    }

    public readonly int TotalScore()
    {
        return Scary.Score + Goofy.Score + Beauty.Score + Anonymity.Score;
    }
}

public class ScoreCalculator : MonoBehaviour
{
    ShopManager _shopManager;

    void Start()
    {
        _shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
    }

    public StatsScore? GetActiveStatsScore(Mask mask = null)
    {
        if (_shopManager == null || _shopManager.CurrentCustomer == null)
            return null;

        if (mask == null)
        {
            var maskDisplay = GameObject.Find("MaskDisplay");
            if (maskDisplay == null)
                return null;
            mask = maskDisplay.GetComponentInChildren<Mask>();
        }

        if (mask == null)
            return null;

        CustomerData customerData = _shopManager.CurrentCustomer.Data;

        var activeMaskParts = MaskPiece.GetActiveMaskPartData(mask);

        var mountPoints = mask.GetComponentsInChildren<MountPoint>();
        int numberOfSlots = mountPoints.Select(mp => Tuple.Create(mp.Type, mp.Handedness)).Distinct().Count();

        StatsScore score = new ();

        score.Scary.Progress = (float)activeMaskParts.Sum(p => p.scaryStat) / numberOfSlots;
        score.Goofy.Progress = (float)activeMaskParts.Sum(p => p.goofyStat) / numberOfSlots;
        score.Beauty.Progress = (float)activeMaskParts.Sum(p => p.beautyStat) / numberOfSlots;
        score.Anonymity.Progress = (float)activeMaskParts.Sum(p => p.anonymityStat) / numberOfSlots;

        score.Scary.InRange = customerData.maskScary.InRange(score.Scary.Progress);
        score.Goofy.InRange = customerData.maskGoofy.InRange(score.Goofy.Progress);
        score.Beauty.InRange = customerData.maskBeauty.InRange(score.Beauty.Progress);
        score.Anonymity.InRange = customerData.maskAnonymity.InRange(score.Anonymity.Progress);

        score.Scary.Score = activeMaskParts.Sum(p => p.scaryStat) * customerData.maskScary.Points;
        score.Goofy.Score = activeMaskParts.Sum(p => p.goofyStat) * customerData.maskGoofy.Points;
        score.Beauty.Score = activeMaskParts.Sum(p => p.beautyStat) * customerData.maskBeauty.Points;
        score.Anonymity.Score = activeMaskParts.Sum(p => p.anonymityStat) * customerData.maskAnonymity.Points;

        return score;
    }
}
