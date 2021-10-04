using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class EnemyTransform
{
	[FormerlySerializedAs("TurretEnemy")]
	public Minion Minion;
	public Vector2 SpawnPosition;
	public int Amount;
}