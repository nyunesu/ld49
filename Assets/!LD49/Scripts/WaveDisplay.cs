using TMPro;
using UnityEngine;

public class WaveDisplay : MonoBehaviour
{
	[SerializeField]
	private TMP_Text waveText;

	public void Display(int waveIndex)
	{
		waveText.SetText($"wave {waveIndex + 1}/{StageManager.GetInstance().WaveAmount}");
	}
}