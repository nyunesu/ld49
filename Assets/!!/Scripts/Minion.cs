using System.Collections;
using System.Collections.Generic;
using ADEStats;
using ADEUtility;
using UnityEngine;

public class Minion : MonoBehaviour
{
	[SerializeField]
	private GraphicsHolder graphicsHolder;

	[SerializeField]
	private string minionName;

	[SerializeField]
	private Stats moveSpeed;
	
	public static readonly List<Minion> Active = new List<Minion>();
	private new Rigidbody2D rigidbody;
	private Health health;
	private bool isFrozen;
	private float unfreezeTime;

	public Vector2 Velocity
	{
		get => rigidbody.velocity;
	}

	private void Awake()
	{
		Active.Add(this);
		TryGetComponent(out rigidbody);
		TryGetComponent(out health);
	}

	private void OnMouseEnter()
	{
		string tooltipText = $"<b>{minionName}</b>";
		GameManager.GetInstance().SetTooltip(true, tooltipText);
	}

	private void OnMouseExit()
	{
		GameManager.GetInstance().SetTooltip(false);
	}

	private void OnEnable()
	{
		health.OnDiedEvent += Die;
		health.OnChangedEvent += _ => graphicsHolder.Flash();
	}

	private void OnDisable()
	{
		health.OnDiedEvent -= Die;
		Active.Remove(this);
	}

	private void FixedUpdate()
	{
		rigidbody.velocity = transform.right * moveSpeed.Value;
	}

	private void Update()
	{
		if (isFrozen && Time.time > unfreezeTime)
		{
			isFrozen = false;
			moveSpeed.RemoveAllModifiers();
		}

		float angle = Helper.GetAngleFromVector(
			transform.position.DirectionTo(Core.GetInstance().transform.position)
		);

		transform.eulerAngles = new Vector3(0, 0, angle);
	}

	public static void ClearActive()
	{
		foreach (Minion minion in Active)
			Destroy(minion.gameObject);

		Active.Clear();
	}

	private void Die()
	{
		Destroy(gameObject);
	}

	public void ApplySlow(StatsModifier modifier)
	{
		moveSpeed.RemoveAllModifiersFromSource(modifier.Source);
		moveSpeed.AddModifier(modifier);
		isFrozen = true;
		unfreezeTime = Time.time + .5f;
	}
}
