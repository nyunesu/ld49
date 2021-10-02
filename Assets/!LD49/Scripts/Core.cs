using ADEUtility;
using UnityEngine;

public class Core : MonoSingleton<Core>
{
	private Health health;

	protected override void Awake()
	{
		base.Awake();
		TryGetComponent(out health);
	}

	private void OnEnable()
	{
		health.OnDiedEvent += Die;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Destroy(other.gameObject);
		health.TakeDamage(1);
	}

	private void Die()
	{
		GameSceneManager.ReloadGame();
	}
}
