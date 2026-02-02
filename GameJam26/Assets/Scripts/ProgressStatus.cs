using System.Linq;
using UnityEngine;
using System;

public class ProgressStatus : MonoBehaviour
{
    public BuildProgressBar BarBeauty;
    public BuildProgressBar BarScary;
    public BuildProgressBar BarGoofy;
    public BuildProgressBar BarAnon;

    ShopManager _shopManager;
    ScoreCalculator _scoreCalculator;

    void Start()
    {
        _shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        _scoreCalculator = Utilities.GetRootComponentRecursive<ScoreCalculator>();
    }

    void Update()
    {
        if ( _shopManager.CurrentCustomer == null )
            return;

        CustomerData customerData = _shopManager.CurrentCustomer.Data;

        var statsScore = _scoreCalculator.GetActiveStatsScore();
        if ( statsScore == null )
            return;

        static void SetBar(BuildProgressBar bar, CustomerStat customerStat, SingleStat currentStat)
        {
            bar.SetArrowMin(customerStat.Min);
            bar.SetArrowMax(customerStat.Max);
            bar.SetMeter(currentStat.Progress);
            bar.SetInRange(currentStat.InRange);
        }

        SetBar(BarBeauty, customerData.maskBeauty, statsScore.Value.Beauty);
        SetBar(BarScary, customerData.maskScary, statsScore.Value.Scary);
        SetBar(BarGoofy, customerData.maskGoofy, statsScore.Value.Goofy);
        SetBar(BarAnon, customerData.maskAnonymity, statsScore.Value.Anonymity);
    }
}
