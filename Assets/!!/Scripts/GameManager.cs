using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADEAudio;
using ADEUtility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
	public TotemInfo SelectedTotem { get; private set; }

	public int TotemCount { get; private set; }

	public int TotemCap { get; private set; } = 1;

	public int Gold
	{
		get => gold;
		set
		{
			gold = value;
			UpdateHud();
		}
	}

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
	private CanvasGroup playGroup;

	[SerializeField]
	private Button playButton;

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

	[SerializeField]
	private int gold;

	[SerializeField]
	private float musicVolume;

	[SerializeField]
	private StageData[] stages;

	[SerializeField]
	private Vector2 gameScreenSize;

	[SerializeField]
	private int stageReward = 5;

	public event Action OnHudChanged;

	public float UpgradeDistance = 2.5f;
	private const int MaxLevel = 7;
	private int level = 1;
	private int stageIndex = -1;
	private bool isShopLocked;
	[FormerlySerializedAs("isPlaying")]
	public bool IsPlaying;

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

		playButton.onClick.AddListener(StartNextStage);
	}

	private void Start()
	{
		Minion.ClearActive();
		MusicManager.SetVolume(musicVolume);
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

		if (Input.GetMouseButtonDown(0) && SelectedTotem)
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

	public void Sell(Totem totem)
	{
		Gold += totem.Info.SellPrice;
		TotemCount--;
		Destroy(totem.gameObject);
		UpdateHud();
	}

	private void EndStage()
	{
		if (stageIndex >= (stages.Length - 1))
		{
			GameSceneManager.LoadNext();
			return;
		}

		IsPlaying = false;
		Gold += stageReward;

		if (!isShopLocked)
			PerformReroll();

		HandleButtonPress();
	}

	private void StartNextStage()
	{
		IsPlaying = true;
		stageIndex++;
		StartCoroutine(StageRoutine(stages[stageIndex]));
		HandleButtonPress();
	}

	private IEnumerator StageRoutine(StageData stage)
	{
		foreach (WaveData stageWave in stage.Waves)
		{
			foreach (EnemyTransform enemySpawn in stageWave.EnemySpawns)
			{
				Instantiate(
					enemySpawn.Minion,
					enemySpawn.SpawnPosition * gameScreenSize,
					Quaternion.identity
				);

				yield return null;
			}

			while (Minion.Active.Count > 0)
				yield return null;
		}

		EndStage();
	}

	private void SelectTotem(TotemInfo totemInfo)
	{
		SelectedTotem = totemInfo;
		SetGhost(true);
		HandleButtonPress();
	}

	private void SetGhost(bool value)
	{
		ghostTotem.gameObject.SetActive(value);

		if (SelectedTotem)
			ghostTotem.Display(SelectedTotem);
	}

	private void TryPlace()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (TotemCount >= TotemCap)
			return;

		Vector3 worldMouse = Helper.GetMouseWorldPosition();

		if (Helper.GetClosestObjectInCircleRadius(worldMouse, UpgradeDistance, out Core _))
			return;

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
			if (totemsInUpgradeRange.All(x => x.Info.Type != SelectedTotem.Type))
				return;

			Totem upgradableTotem = totemsInUpgradeRange.First(x => x.Info.Type == SelectedTotem.Type);

			if (upgradableTotem && upgradableTotem.Info.Upgrade)
				upgradableTotem.Upgrade();
			else
				return;
		}
		else
		{
			Totem totemInstance = Instantiate(SelectedTotem.TotemPrefab, worldMouse, Quaternion.identity);
			totemInstance.Setup(SelectedTotem);
			TotemCount++;
		}

		SelectedTotem = null;
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
		SetTooltip(true, $"Increase Level by 1\nIncreases totem maximum amount\n(${levelUpCost})");
	}

	private void UpdateHud()
	{
		shopGroup.interactable = !IsPlaying && !SelectedTotem;
		totemDescription.SetText($"Totem [{TotemCount}/{TotemCap}]");
		waveCount.SetText($"Stage [{stageIndex}/{stages.Length}]");
		bool isMaxLevel = level >= MaxLevel;
		levelButton.interactable = !IsPlaying && !isMaxLevel && Gold >= levelUpCost;
		levelButtonDescription.SetText(isMaxLevel ? "MAX LEVEL" : $"Lv.{level}");
		rerollButton.interactable = !IsPlaying && Gold >= rerollCost;
		goldDescription.SetText($"${Gold}");
		playGroup.alpha = IsPlaying ? 0 : 1;
		playGroup.interactable = !IsPlaying;
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
