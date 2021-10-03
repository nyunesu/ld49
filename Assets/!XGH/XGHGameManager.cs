using System;
using System.Collections.Generic;
using System.Linq;
using ADEUtility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XGHGameManager : MonoSingleton<XGHGameManager>
{
	[SerializeField]
	private TMP_Text totemDescription;

	[SerializeField]
	private TMP_Text waveCount;

	[SerializeField]
	private RectTransform tooltip;

	[SerializeField]
	private LayoutElement tooltipLayoutElement;

	[SerializeField]
	private int tooltipCharacterLimit = 80;

	[SerializeField]
	private float tooltipOffset = -64;

	[SerializeField]
	private TMP_Text tooltipDescription;

	[SerializeField]
	private TMP_Text goldDescription;

	[SerializeField]
	private CanvasGroup shopGroup;

	[SerializeField]
	private int shopItemAmount = 3;

	[SerializeField]
	private Transform totemShopContainer;

	[SerializeField]
	private TotemButton totemShopButtonPrefab;

	[SerializeField]
	private Button rerollButton;

	[SerializeField]
	private int rerollCost = 1;

	[SerializeField]
	private Button lockButton;

	[SerializeField]
	private Button levelButton;

	[SerializeField]
	private TMP_Text levelButtonDescription;

	[SerializeField]
	private GhostTotem ghostTotem;

	[SerializeField]
	private int levelUpCost = 2;

	public event Action OnHudChanged;

	public float UpgradeDistance = 2.5f;
	public WaveData[] Waves;
	public int Gold;
	public int TotemCount;
	public int TotemCap = 1;
	private const int MaxLevel = 7;
	private int level = 1;
	private int waveIndex;
	private bool isShopLocked;
	private TotemInfo selectedTotem;

	protected override void Awake()
	{
		base.Awake();
		rerollButton.onClick.AddListener(BuyReroll);

		rerollButton.gameObject.AddComponent<PointerCallback>()
			.SetCallback(DisplayRerollTooltip, () => SetTooltip(false));

		lockButton.onClick.AddListener(ToggleShopLock);

		lockButton.gameObject.AddComponent<PointerCallback>()
			.SetCallback(DisplayLockTooltip, () => SetTooltip(false));

		levelButton.onClick.AddListener(LevelUp);

		levelButton.gameObject.AddComponent<PointerCallback>()
			.SetCallback(DisplayLevelTooltip, () => SetTooltip(false));
	}

	private void Start()
	{
		SetTooltip(false);
		SetGhost(false);
		PerformReroll();
		UpdateHud();
	}

	private void Update()
	{
		if (tooltip.gameObject.activeSelf)
		{
			tooltip.transform.position = Input.mousePosition;

			var pivot = new Vector2(
				Input.mousePosition.x / Screen.width,
				Input.mousePosition.y / Screen.height
			);

			tooltip.pivot = pivot;
			tooltip.transform.position += (Vector3) (pivot - (Vector2.one * .5f)) * tooltipOffset;
		}

		if (Input.GetMouseButtonDown(0) && selectedTotem)
			TryPlace();
	}

	public void SetTooltip(bool value, string context = "")
	{
		tooltip.gameObject.SetActive(value);
		tooltipDescription.SetText(context);
		tooltipLayoutElement.enabled = tooltipDescription.text.Length > tooltipCharacterLimit;
	}

	public void Purchase(TotemInfo totemInfo)
	{
		Gold -= totemInfo.Cost;
		SelectTotem(totemInfo);
	}

	public void SelectTotem(TotemInfo totemInfo)
	{
		selectedTotem = totemInfo;
		SetGhost(true);
		HandleButtonPress();
	}

	private void SetGhost(bool value)
	{
		ghostTotem.gameObject.SetActive(value);

		if (selectedTotem)
			ghostTotem.Display(selectedTotem);
	}

	private void TryPlace()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (TotemCount >= TotemCap)
			return;

		Vector3 worldMouse = Helper.GetMouseWorldPosition();

		Helper.GetAllObjectsInCircleRadius(
			worldMouse,
			UpgradeDistance,
			out List<Totem> totemsInUpgradeRange
		);

		totemsInUpgradeRange = totemsInUpgradeRange.Where(
				x => Vector2.Distance(x.transform.position, worldMouse) < UpgradeDistance
			)
			.ToList();

		if (totemsInUpgradeRange.Count > 0)
		{
			Totem upgradableTotem = totemsInUpgradeRange.First(x => x.Info.Type == selectedTotem.Type);

			if (upgradableTotem)
				upgradableTotem.Upgrade();
			else
				return;
		}
		else
		{
			Totem totemInstance = Instantiate(selectedTotem.TotemPrefab, worldMouse, Quaternion.identity);
			totemInstance.Setup(selectedTotem);
			TotemCount++;
		}

		selectedTotem = null;
		SetGhost(false);
		HandleButtonPress();
	}

	private void DisplayLockTooltip()
	{
		string tooltipText = isShopLocked
			? "Unlocks current selection of shop items"
			: "Locks current selection of shop items";

		SetTooltip(true, tooltipText);
	}

	private void DisplayRerollTooltip()
	{
		SetTooltip(
			true,
			$"Replace current selection of shop items\nwith new randomized options\n(${rerollCost})"
		);
	}

	private void DisplayLevelTooltip()
	{
		SetTooltip(true, $"Increase Level by 1\n(${levelUpCost})");
	}

	private void UpdateHud()
	{
		shopGroup.interactable = !selectedTotem;
		totemDescription.SetText($"Totem [{TotemCount}/{TotemCap}]");
		waveCount.SetText($"Wave [{waveIndex}/{Waves.Length}]");
		bool isMaxLevel = level >= MaxLevel;
		levelButton.interactable = !isMaxLevel && Gold >= levelUpCost;
		levelButtonDescription.SetText(isMaxLevel ? "MAX LEVEL" : $"Lv.{level}");
		rerollButton.interactable = Gold >= rerollCost;
		goldDescription.SetText($"${Gold}");
		OnHudChanged?.Invoke();
	}

	private void ToggleShopLock()
	{
		isShopLocked = !isShopLocked;
		HandleButtonPress();
	}

	private void HandleButtonPress()
	{
		SetTooltip(false);
		UpdateHud();
	}

	private void BuyReroll()
	{
		PerformReroll();
		Gold -= rerollCost;
		isShopLocked = false;
		HandleButtonPress();
	}

	private void PerformReroll()
	{
		foreach (Transform item in totemShopContainer)
			Destroy(item.gameObject);

		for (int i = 0; i < shopItemAmount; i++)
			Instantiate(totemShopButtonPrefab, totemShopContainer);
	}

	private void LevelUp()
	{
		Gold -= levelUpCost;
		level++;
		TotemCap++;
		HandleButtonPress();
	}
}
