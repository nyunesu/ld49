using System;
using System.Collections;
using ADEStats;
using UnityEngine;

public class Health : MonoBehaviour
{
	public bool IsInvulnerable { get; private set; }

	public float NormalizedValue
	{
		get => stats.NormalizedValue;
	}

	[SerializeField]
	private Stats stats;

	public event Action<float> OnChangedEvent;

	public event Action OnDiedEvent;

	private void OnEnable()
	{
		stats.OnMinimumReached += Die;
		stats.OnChangedEvent += delta => OnChangedEvent?.Invoke(delta);
	}

	private void OnDisable()
	{
		stats.OnMinimumReached -= Die;
	}

	public void TakeDamage(float amount)
	{
		if (IsInvulnerable)
			return;

		stats.Value -= amount;
	}

	public void Restore(float amount)
	{
		stats.Value += amount;
	}

	public void SetInvulnerable(float duration = .3f)
	{
		StartCoroutine(InvulnerableRoutine(duration));
	}

	public void SetMaxHealth(float value)
	{
		stats.RemoveAllModifiers();
		stats.SetMax(value);
	}

	private IEnumerator InvulnerableRoutine(float duration)
	{
		IsInvulnerable = true;
		yield return new WaitForSeconds(duration);

		IsInvulnerable = false;
	}

	private void Die()
	{
		OnDiedEvent?.Invoke();
	}
}