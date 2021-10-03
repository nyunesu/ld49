using System.Collections.Generic;
using ADEUtility;
using UnityEngine;

public class Minion : MonoBehaviour
{
	[SerializeField]
	private GraphicsHolder graphicsHolder;

	public static readonly List<Minion> Active = new List<Minion>();
	private new Rigidbody2D rigidbody;
	private Health health;

	private void Awake()
	{
		Active.Add(this);
		TryGetComponent(out rigidbody);
		TryGetComponent(out health);
	}

	private void OnEnable()
	{
		health.OnDiedEvent += Die;
		health.OnChangedEvent += _ => graphicsHolder.Flash();
	}

	private void OnDisable()
	{
		health.OnDiedEvent -= Die;
	}

	private void OnDestroy()
	{
		Active.Remove(this);
	}

	private void FixedUpdate()
	{
		rigidbody.velocity = transform.right;
	}

	private void Update()
	{
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
}
