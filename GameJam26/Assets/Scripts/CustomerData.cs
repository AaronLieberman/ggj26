using System;
using System.Collections.Generic;
using UnityEngine;

public struct CustomerStat
{
    public int Min;
    public int Max;
    public int Points;

    public readonly bool InRange(float value)
    {
        float extraTolerance = 0.25f;
        return (value + extraTolerance) >= Min && (value - extraTolerance) <= Max;
    }
}

public struct CustomerTag
{
    public string TagName;
    public int Min;
    public int Max;
    public int Points;
}

public class CustomerData
{
    public string customerName;
    public string customerDialogue;
    public string customerImageName;
    public string customMaskPrefab;
    public Sprite customerImage;
    public string maskBase;
    public float time;
    public int difficultyTier;
    public CustomerStat maskScary;
    public CustomerStat maskGoofy;
    public CustomerStat maskBeauty;
    public CustomerStat maskAnonymity;
    public List<CustomerTag> minimumTags = new();
    public List<CustomerTag> maximumTags = new();
    public int pointsPerTag;
    public int maxScore;
    public int gradeAThreshold;
    public int gradeBThreshold;
    public int gradeCThreshold;
    public int gradeDThreshold;

    public string gradeADialog;
    public string gradeBDialog;
    public string gradeCDialog;
    public string gradeDDialog;
    public string gradeFDialog;
}
