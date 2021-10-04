using UnityEngine;

[CreateAssetMenu(menuName = "Totem", fileName = "Totem Info")]
public class TotemInfo : ScriptableObject
{
	public int SellPrice
	{
		get => Cost - 1;
	}

	public float ProjectileSpeed;
	public int Damage;
	public float FireInterval;
	public Projectile Projectile;
	public float MaxHeat;
	public float HeatPerShot;
	public float Range = 2;
	public int Cost;
	public string DisplayName;
	public Sprite Sprite;
	public Sprite Icon;
	public Totem TotemPrefab;
	public TotemType Type;
	public TotemUpgrade Upgrade;

	[TextArea]
	public string Description;

	public string GetHoverDescription(int points)
	{
		if (GameManager.GetInstance().SelectedTotem != null)
			return GetUpgradeDescription(GameManager.GetInstance().SelectedTotem, points);

		string description = $"<b>{DisplayName}</b>\n<i>{Description}</i>";

		if (Upgrade != null)
			description += $"\n\n{DisplayName} -> {Upgrade.Info.DisplayName}\n[{points}/{Upgrade.Cost}]";

		description += $"\n\n<b>{Type}</b>";
		description += $"\n<i>Double click to sell (${SellPrice})</i>";
		return description;
	}

	public string GetShopDescription()
	{
		string description = $"<b>${Cost}</b>";
		description += $"\n<b>{DisplayName}</b>\n<i>{Description}</i>";
		description += $"\n\n<b>{Type}</b>";
		return description;
	}

	private string GetUpgradeDescription(TotemInfo selectedTotem, int points)
	{
		string description = string.Empty;

		if (selectedTotem.Type == Type)
		{
			if (Upgrade == null)
				description = "No upgrades available";
			else
				description
					+= $"<b>{DisplayName} -> {Upgrade.Info.DisplayName}</b>\n[{points}/{Upgrade.Cost}]";
		}
		else
			description += $"You can't upgrade <b>{Type}</b> with <b>{selectedTotem.Type}</b>";

		return description;
	}
}

public enum TotemType
{
	Glacial,
	Tempest,
	Infernal,
	Ancient,
}
