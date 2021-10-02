using System.Collections.Generic;
using ADEUtility;
using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
	[SerializeField]
	private GraphicsHolder graphicsHolder;

	public static readonly List<TurretEnemy> ActiveEnemies = new List<TurretEnemy>();
	private new Rigidbody2D rigidbody;
	private Health health;

	private void Awake()
	{
		ActiveEnemies.Add(this);
		TryGetComponent(out rigidbody);
		TryGetComponent(out health);
	}

	private void Start()
	{
		graphicsHolder.Bounce();
	}

	private void OnEnable()
	{
		health.OnDiedEvent += Die;
	}

	private void OnDestroy()
	{
		ActiveEnemies.Remove(this);
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

	public static void KillActiveEnemies()
	{
		foreach (TurretEnemy activeEnemy in ActiveEnemies)
			activeEnemy.Die();
	}

	private void Die()
	{
		Destroy(gameObject);
	}
}
