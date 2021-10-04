using ADEStats;
using UnityEngine;

public class FreezeProjectiles : Projectile
{
	[SerializeField]
	private float slowPercentage = .3f;
	
	protected override void HitEffect(Health health)
	{
		health.TakeDamage(Damage);

		if (health.TryGetComponent(out Minion minion))
			minion.ApplySlow(new StatsModifier(slowPercentage, StatsModifierType.Multiplier, Source));
	}
}
