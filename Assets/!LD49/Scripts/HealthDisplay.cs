using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	[SerializeField]
	private Image fill;

	[SerializeField]
	private Health health;

	private void Start()
	{
		Display();
	}

	private void OnEnable()
	{
		health.OnChangedEvent += Display;
	}

	private void OnDisable()
	{
		health.OnChangedEvent -= Display;
	}

	private void Display(float delta)
	{
		Display();
	}

	private void Display()
	{
		if (!health)
			return;

		fill.fillAmount = health.NormalizedValue;
	}
}