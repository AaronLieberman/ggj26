using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MaskPieceScriptableObject", menuName = "Scriptable Objects/MaskPieceScriptableObject")]
public class MaskPieceScriptableObject : ScriptableObject
{
    public string maskPieceName;
    public MaskPieceType maskPieceType;
	public Sprite maskPieceSprite;
	public int scaryStat;
	public int goofyStat;
	public int beautyStat;
	public List<string> maskTags;
}
