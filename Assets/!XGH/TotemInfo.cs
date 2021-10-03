using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Totem", fileName = "Totem Info")]
public class TotemInfo : ScriptableObject
{
	public int Cost;
	public string Name;
	public Sprite Sprite;
	public Sprite Icon;
	public Totem TotemPrefab;
	public TotemType Type;
	public TotemUpgrade Upgrade;
}

public enum TotemType
{
	Frost, Lightning, Flame,
}

[Serializable]
public class TotemUpgrade
{
	public int Cost;
	public TotemInfo Upgrade;
}
