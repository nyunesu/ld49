using System.Collections.Generic;
using System.Linq;
using ADEStats;
using ADEUtility;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
	[SerializeField]
	private Projectile projectilePrefab;

	[SerializeField]
	private Transform firePoint;

	[SerializeField]
	private Transform firePointPivot;

	[SerializeField]
	private float fireCooldown = 0.65f;

	[SerializeField]
	private float projectileSpeed = 24f;

	[SerializeField]
	private Stats heat;

	[SerializeField]
	private Image heatBar;

	[SerializeField]
	private Gradient heatGradient;

	[SerializeField]
	private float heatPerShot = .2f;

	[SerializeField]
	private GraphicsHolder graphicsHolder;

	[SerializeField]
	private LineRenderer linkLineRendererPrefab;

	public float SightDistance = 5f;
	public TurretData TurretData;
	private readonly HashSet<Turret> turretLinks = new HashSet<Turret>();
	private readonly List<LineRenderer> lineRenderers = new List<LineRenderer>();
	private float nextFireTime;
	private CinemachineImpulseSource shakeSource;
	private TurretEnemy target;
	private bool overheat;
	private int amountToUpgrade;

	private void Awake()
	{
		amountToUpgrade = TurretData.AmountToUpgrade;
	}

	private void OnEnable()
	{
		heat.OnMaximumReached += () => SetOverheat(true);
		heat.OnMinimumReached += () => SetOverheat(false);
		heat.OnChangedEvent += UpdateHeatUI;
		TurretSpawner.GetInstance().OnTurretsChanged += CalculateLinks;
	}

	private void Update()
	{
		UpdateHeatMeter();

		if (!target && !Helper.GetClosestObjectInCircleRadius(transform.position, SightDistance, out target))
			return;

		if (overheat || turretLinks.Any(x => x.overheat))
			return;

		if (Vector2.Distance(target.transform.position, transform.position) > SightDistance)
		{
			target = null;
			return;
		}

		LookToTarget();
		TryFire();
	}

	public void AddUpgrade()
	{
		if (amountToUpgrade <= 0)
			return;

		amountToUpgrade--;

		if (amountToUpgrade <= 0)
			Upgrade();
	}

	private void Upgrade()
	{
		TurretSpawner.GetInstance().Spawn(TurretData.Upgraded, transform);
		Die();
	}

	private void Die()
	{
		TurretSpawner.GetInstance().DestroyTurret(this);
	}

	private void SetOverheat(bool value)
	{
		overheat = value;

		if (value)
		{
			graphicsHolder.SetColor(ColorHelper.DarkGray);
			graphicsHolder.Blink();

			foreach (LineRenderer lineRenderer in lineRenderers)
				lineRenderer.startColor = lineRenderer.endColor = ColorHelper.DarkGray;
		}
		else
		{
			graphicsHolder.SetColor(ColorHelper.WhiteGray);
			graphicsHolder.Bounce();

			foreach (LineRenderer lineRenderer in lineRenderers)
				lineRenderer.startColor = lineRenderer.endColor = ColorHelper.WhiteGray;
		}
	}

	private void CalculateLinks()
	{
		turretLinks.Clear();

		foreach (LineRenderer lineRenderer in lineRenderers)
			Destroy(lineRenderer);

		lineRenderers.Clear();

		Helper.GetAllObjectsInCircleRadius(
			transform.position,
			GameManager.LinkDistance,
			out List<Turret> turrets
		);

		turrets.Remove(this);

		foreach (Turret turret in turrets)
		{
			turretLinks.Add(turret);
			LineRenderer linkLine = Instantiate(linkLineRendererPrefab);
			lineRenderers.Add(linkLine);
			linkLine.SetPosition(0, transform.position);
			linkLine.SetPosition(1, turret.transform.position);
		}
	}

	private void UpdateHeatMeter()
	{
		heat.Remove(Time.deltaTime);
	}

	private void UpdateHeatUI(float value)
	{
		heatBar.fillAmount = heat.NormalizedValue;
		heatBar.color = heatGradient.Evaluate(heatBar.fillAmount);
	}

	private void TryFire()
	{
		if (nextFireTime > Time.time)
			return;

		heat.Value += heatPerShot;
		SoundEffects.Play(SoundType.BowStringRelease);
		Quaternion firePointRotation = firePoint.rotation;
		Vector3 firePointPosition = firePoint.position;
		Projectile projectile = Instantiate(projectilePrefab, firePointPosition, firePointRotation);
		projectile.Speed = projectileSpeed;
		projectile.Damage = 1;
		nextFireTime = Time.time + fireCooldown;
		Helper.Shake(1.5f, .25f, .0625f, target.transform.position.DirectionTo(transform.position));
	}

	private void LookToTarget()
	{
		float angleToTarget
			= Helper.GetAngleFromVector(transform.position.DirectionTo(target.transform.position));

		Quaternion targetLookRotation = Quaternion.Euler(0, 0, angleToTarget);
		firePointPivot.transform.rotation = targetLookRotation;
	}
}
