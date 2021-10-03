using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave", order = 0)]
public class WaveData : ScriptableObject
{
	public List<EnemyTransform> EnemySpawns = new List<EnemyTransform>();
}