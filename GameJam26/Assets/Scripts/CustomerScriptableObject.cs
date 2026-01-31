using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public struct StringIntPair
{
	public string key;
	public int value;
}

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
    public List<StringIntPair> maskGoalTagCount;
}

// [CreateAssetMenu(fileName = "CustomerScriptableObject", menuName = "Scriptable Objects/CustomerScriptableObject")]
// public class CustomerScriptableObject : ScriptableObject
// {
//     public string customerName;
//     public string customerDialogue;
//     public string customerImageName;
//     public Sprite customerImage;
//     public CustomerStat maskScary;
//     public CustomerStat maskGoofy;
//     public CustomerStat maskBeauty;
//     public CustomerStat maskAnonymity;
//     public List<StringIntPair> maskGoalTagCount;
// }
