using ADEUtility;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField]
	private StageManager stageManager;

	public static int LinkDistance = 4;
	public static float UpgradeDistance = 1.5f;

	private void Start()
	{
		StartCoroutine(stageManager.StageRoutine());
	}
}
