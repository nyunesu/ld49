using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Stage", order = 0)]
public class StageData : ScriptableObject
{
	public List<WaveData> Waves = new List<WaveData>();
}
