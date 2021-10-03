using ADEUtility;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField]
	private StageManager stageManager;

	public static int LinkDistance = 4;

	private void Start()
	{
		StartCoroutine(stageManager.StageRoutine());
	}
}
