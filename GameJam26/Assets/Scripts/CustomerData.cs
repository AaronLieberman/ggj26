using System.Collections.Generic;
using UnityEngine;

public struct CustomerStat
{
    public int Min;
    public int Max;
    public int Points;
}

public class CustomerData
{
    public string customerName;
    public string customerDialogue;
    public string customerImageName;
    public Sprite customerImage;
    public CustomerStat maskScary;
    public CustomerStat maskGoofy;
    public CustomerStat maskBeauty;
    public CustomerStat maskAnonymity;
    // public List<StringIntPair> maskGoalTagCount;
}
