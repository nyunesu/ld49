using UnityEngine;

[CreateAssetMenu(menuName = "!/Turret", fileName = "Turret")]
public class TurretData : ScriptableObject
{
	public Turret Prefab;
	public Ghost GhostPrefab;
	public Sprite Sprite;
	public int Cost;
	public TurretData Upgraded;
	public int AmountToUpgrade;
}
