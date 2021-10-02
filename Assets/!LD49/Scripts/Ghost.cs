using UnityEngine;

public class Ghost : MonoBehaviour
{
	[SerializeField]
	private Transform connectRange;

	[SerializeField]
	private Transform sightRange;

	[SerializeField]
	private Transform upgradeRange;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, GameManager.LinkDistance);
		Gizmos.DrawWireSphere(transform.position, GameManager.UpgradeDistance);
	}

	public void Display(TurretData turretData)
	{
		// sightRange.localScale = Vector3.one * turretData.Prefab.SightDistance;
		// connectRange.localScale = Vector3.one * GameManager.LinkDistance;
		// upgradeRange.localScale = Vector3.one * GameManager.UpgradeDistance;
	}
}
