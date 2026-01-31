using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public struct StringIntPair
{
	public string key;
	public int value;
}

[CreateAssetMenu(fileName = "CustomerScriptableObject", menuName = "Scriptable Objects/CustomerScriptableObject")]
public class CustomerScriptableObject : ScriptableObject
{
    public string customerName;
    public string customerDialogue;
    public Sprite customerImage;
    public int maskGoalScaryLevel;
    public int maskGoalGoofyLevel;
    public int maskGoalBeautyLevel;
    public int maskGoalAnonymityLevel;
    public List<StringIntPair> maskGoalTagCount;
}
