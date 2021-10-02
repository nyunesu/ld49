using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Wave Data", order = 0)]
public class WaveData : ScriptableObject
{
	public List<EnemyTransform> EnemySpawns = new List<EnemyTransform>();
}