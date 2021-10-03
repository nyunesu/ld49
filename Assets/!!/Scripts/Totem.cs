using ADEStats;
using ADEUtility;
using UnityEngine;
using UnityEngine.UI;

public class Totem : MonoBehaviour
{
	[SerializeField]
	private Image heatBar;

	[SerializeField]
	private Transform firePoint;

	[SerializeField]
	private Gradient heatGradient;

	[SerializeField]
	private ParticleSystem muzzleFx;

	public TotemInfo Info;
	private Stats heat;
	private int points;
	private float lastClickTime;
	private bool isOverheated;
	private Minion target;
	private float nextFireTime;

	private void Update()
	{
		UpdateHeatMeter();

		if (isOverheated)
			return;

		if (!target && !Helper.GetClosestObjectInCircleRadius(transform.position, Info.Range, out target))
			return;

		if (Vector2.Distance(target.transform.position, transform.position) > Info.Range)
		{
			target = null;
			return;
		}

		LookToTarget();
		TryFire();
	}

	private void OnMouseDown()
	{
		GameManager.GetInstance().SetTooltip(false);

		if (GameManager.GetInstance().SelectedTotem || GameManager.GetInstance().IsPlaying)
			return;

		if ((Time.time - lastClickTime) < 0.25f)
			Sell();

		lastClickTime = Time.time;
	}

	private void OnMouseEnter()
	{
		GameManager.GetInstance().SetTooltip(true, Info.GetHoverDescription(points));
	}

	private void OnMouseExit()
	{
		GameManager.GetInstance().SetTooltip(false);
	}

	public void Setup(TotemInfo totemInfo)
	{
		points = 0;
		heat = new Stats(0, Info.MaxHeat, 0);
		heat.OnMaximumReached += () => isOverheated = true;
		heat.OnMinimumReached += () => isOverheated = false;
		Info = totemInfo;
		heatBar.transform.parent.SetParent(null);
	}

	public void Upgrade(int pointIncrement = 1)
	{
		points += pointIncrement;

		if (Info.Upgrade == null || points < Info.Upgrade.Cost)
			return;

		Setup(Info.Upgrade.Info);
	}

	private void UpdateHeatMeter()
	{
		heat.Remove(Time.deltaTime);
		heatBar.fillAmount = heat.NormalizedValue;
		heatBar.color = heatGradient.Evaluate(heatBar.fillAmount);
	}

	private void LookToTarget()
	{
		float angleToTarget
			= Helper.GetAngleFromVector(transform.position.DirectionTo(target.transform.position));

		Quaternion targetLookRotation = Quaternion.Euler(0, 0, angleToTarget);
		transform.rotation = targetLookRotation;
	}

	private void TryFire()
	{
		if (nextFireTime > Time.time)
			return;

		heat.Value += Info.HeatPerShot;
		Instantiate(Info.Projectile, firePoint.position, firePoint.rotation);
		nextFireTime = Time.time + Info.FireInterval;
		Helper.PlayParticleOnce(muzzleFx, firePoint);
		SoundEffects.Play(SoundType.BowStringRelease);
		Helper.Shake(2.5f, .25f, .2f, target.transform.position.DirectionTo(transform.position));
	}

	private void Sell()
	{
		Destroy(heatBar.transform.parent.gameObject);
		GameManager.GetInstance().Sell(this);
	}
}
