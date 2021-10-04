using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADEAudio;
using ADEUtility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
	public TotemInfo SelectedTotem { get; private set; }

	public int TotemCount { get; private set; }

	public int TotemCap { get; private set; } = 1;

	public int Gold
	{
		get => gold;
		private set
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
	private Button levelButton;

	[SerializeField]
	private TMP_Text levelButtonDescription;

	[SerializeField]
	private GhostTotem ghostTotem;

	[SerializeField]
	private GameObject upgradePlane;

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

	[SerializeField]
	private float spawnOffset = .1f;

	[SerializeField]
	private int[] levelUpgradeCosts;

	public event Action OnHudChanged;

	private int levelUpCost
	{
		get => levelUpgradeCosts[level - 1];
	}

	public float UpgradeDistance = 1f;

	[FormerlySerializedAs("isPlaying")]
	public bool IsPlaying;

	private int level = 1;
	private int stageIndex = -1;
	private bool isShopLocked;
	private Coroutine stageRoutine;
	private static GameManager instance;
	private bool gameEnded = false;

	protected void Awake()
	{
		instance = this;
		
		if (SceneManager.GetActiveScene().buildIndex > 0)
			return;

		if (!levelButton)
			return;
		
		levelButton.onClick.AddListener(LevelUp);

		levelButton.gameObject.AddComponent<PointerCallback>()
			.SetCallback(DisplayLevelTooltip, () => SetTooltip(false));

		playButton.onClick.AddListener(StartNextStage);
	}

	private void Start()
	{
		if (SceneManager.GetActiveScene().buildIndex > 0 || gameEnded)
			return;
		
		Minion.ClearActive();
		MusicManager.SetVolume(musicVolume);
		SetTooltip(false);
		SetGhost(false);
		UpdateHud();
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex > 0 || gameEnded)
			return;
		
		if (SelectedTotem && Input.GetMouseButtonDown(1))
			Deselect();
		
		if (Input.GetKeyDown(KeyCode.R))
			Reset();

		if (Input.GetKeyDown(KeyCode.F6))
			EndGame();
		
		if (tooltip != null && tooltip.gameObject.activeSelf)
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
		if (!tooltip)
		{
			Reset();
			return;
		}

		tooltip.gameObject.SetActive(value);
		tooltipDescription.SetText(context);
		tooltipLayoutElement.enabled = tooltipDescription.text.Length > tooltipCharacterLimit;
	}

	public void Select(TotemInfo totemInfo)
	{
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
			EndGame();
			return;
		}

		IsPlaying = false;
		Gold += stageReward;
		HandleButtonPress();
	}

	private void EndGame()
	{
		if (stageRoutine != null)
			StopCoroutine(stageRoutine);

		gameEnded = true;
		SceneManager.LoadScene(1);
	}

	private void StartNextStage()
	{
		IsPlaying = true;
		stageIndex++;
		stageRoutine = StartCoroutine(StageRoutine(stages[stageIndex]));
		HandleButtonPress();
	}

	private IEnumerator StageRoutine(StageData stage)
	{
		foreach (WaveData stageWave in stage.Waves)
		{
			foreach (EnemyTransform enemySpawn in stageWave.EnemySpawns)
			{
				for (int i = 0; i < enemySpawn.Amount; i++)
				{
					Instantiate(
						enemySpawn.Minion,
						(enemySpawn.SpawnPosition * gameScreenSize) + (Random.insideUnitCircle * spawnOffset),
						Quaternion.identity
					);

					yield return null;
				}

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
			{
				upgradableTotem.Upgrade();
				Gold -= SelectedTotem.Cost;
			}
			else
				return;
		}
		else
		{
			if (TotemCount >= TotemCap)
			{
				Deselect();
				return;
			}

			Totem totemInstance = Instantiate(SelectedTotem.TotemPrefab, worldMouse, Quaternion.identity);
			Gold -= SelectedTotem.Cost;
			totemInstance.Setup(SelectedTotem);
			TotemCount++;
		}

		Deselect();
	}

	private void Deselect()
	{
		SelectedTotem = null;
		SetGhost(false);
		HandleButtonPress();
	}

	private void DisplayLevelTooltip()
	{
		SetTooltip(true, "Level +1\nTotem Amount +1");
	}

	private void UpdateHud()
	{
		shopGroup.interactable = !IsPlaying && !SelectedTotem;
		totemDescription.SetText($"Totem [{TotemCount}/{TotemCap}]");
		waveCount.SetText($"Stage [{stageIndex + 1}/{stages.Length}]");
		bool isMaxLevel = level > levelUpgradeCosts.Length;
		levelButton.interactable = !IsPlaying && !isMaxLevel && Gold >= levelUpCost;
		levelButtonDescription.SetText(isMaxLevel ? "MAX LEVEL" : $"Lv ({level}/{levelUpgradeCosts.Length + 1})");
		if (!isMaxLevel)
			levelButtonDescription.text += $"\n(${levelUpCost})";
		goldDescription.SetText($"${Gold}");
		playGroup.alpha = IsPlaying ? 0 : 1;
		playGroup.interactable = !IsPlaying;
		upgradePlane.SetActive(SelectedTotem && TotemCount >= TotemCap);
		OnHudChanged?.Invoke();
	}

	private void HandleButtonPress()
	{
		SetTooltip(false);
		UpdateHud();
	}

	private void LevelUp()
	{
		Gold -= levelUpCost ;
		level++;
		TotemCap++;
		HandleButtonPress();
	}

	public void Reset()
	{
		if (stageRoutine != null)
			StopCoroutine(stageRoutine);
		
		SceneManager.LoadScene(0);
	}
}
