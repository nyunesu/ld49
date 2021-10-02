using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADEUtility;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
	public int WaveAmount
	{
		get => waves.Length;
	}

	[SerializeField]
	private float alertDuration;

	[SerializeField]
	private Vector2 spawnOffset = new Vector2(14, 7);

	[SerializeField]
	private WaveData[] waves;

	public bool Victory;
	public int WaveIndex = -1;
	private Coroutine wavesRoutine;

	protected override void Awake()
	{
		base.Awake();
		Reset();
	}

	public void Reset()
	{
		Victory = false;
	}

	public IEnumerator StageRoutine()
	{
		WaveIndex = 0;

		while (WaveIndex <= (waves.Length - 1))
			yield return SpawnWaveRoutine(WaveIndex);
	}

	private IEnumerator SpawnWaveRoutine(int index)
	{
		WaveData waveData = waves[index];

		foreach (EnemyTransform enemySpawn in waveData.EnemySpawns)
		{
			Instantiate(
				enemySpawn.TurretEnemy,
				enemySpawn.SpawnPosition * spawnOffset,
				Quaternion.identity
			);

			yield return null;
		}

		while (TurretEnemy.ActiveEnemies.Count > 0)
			yield return null;
	}

	private IEnumerator AlertWaveRoutine(WaveData wave)
	{
		IEnumerable<Vector2> distinctSpawnPoints
			= wave.EnemySpawns.Select(x => x.SpawnPosition).Distinct();

		yield return new WaitForSeconds(alertDuration);
	}
}