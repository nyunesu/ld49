using ADEUtility;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem deathFX;

	public float Speed;
	public int Damage;
	private new Rigidbody2D rigidbody;

	private void Awake()
	{
		TryGetComponent(out rigidbody);
	}

	private void Start()
	{
		Destroy(gameObject, 10f);
	}

	private void Update()
	{
		rigidbody.velocity = transform.right * Speed;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent(out Health health))
			health.TakeDamage(Damage);

		Die();
	}

	private void Die()
	{
		if (deathFX)
			Helper.PlayParticleOnce(deathFX, transform);
		
		Destroy(gameObject);
	}
}
