using ADEUtility;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
	public object Source { get; set; }

	[SerializeField]
	private ParticleSystem deathFX;

	public float Speed;
	public int Damage;
	private new Rigidbody2D rigidbody;
	private bool isDead;

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
		if (isDead)
			return;

		if (other.TryGetComponent(out Health health))
			HitEffect(health);

		Die();
	}

	public void Setup(float speed, int damage, object source)
	{
		Speed = speed;
		Damage = damage;
		Source = source;
	}

	protected virtual void HitEffect(Health health)
	{
		health.TakeDamage(Damage);
	}

	private void Die()
	{
		isDead = true;

		if (deathFX)
			Helper.PlayParticleOnce(deathFX, transform);

		Destroy(gameObject);
	}
}
