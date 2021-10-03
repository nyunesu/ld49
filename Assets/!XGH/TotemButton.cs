using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TotemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private TotemInfo totemInfo;

	[SerializeField]
	private Image icon;

	private Button button;

	public void OnPointerEnter(PointerEventData eventData)
	{
		string tooltipText = $"{totemInfo.Name}\n${totemInfo.Cost}";
		XGHGameManager.GetInstance().SetTooltip(true, tooltipText);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		XGHGameManager.GetInstance().SetTooltip(false);
	}

	private void Awake()
	{
		TryGetComponent(out button);
		button.onClick.AddListener(Purchase);
		icon.sprite = totemInfo.Icon;
	}

	private void Start()
	{
		Refresh();
	}

	private void OnEnable()
	{
		XGHGameManager.GetInstance().OnHudChanged += Refresh;
	}

	private void OnDisable()
	{
		XGHGameManager.GetInstance().OnHudChanged -= Refresh;
	}

	private void Purchase()
	{
		XGHGameManager.GetInstance().Purchase(totemInfo);
		Destroy(gameObject);
	}

	private void Refresh()
	{
		var gameManager = XGHGameManager.GetInstance();
		button.interactable = XGHGameManager.GetInstance().Gold >= totemInfo.Cost && gameManager.TotemCount < gameManager.TotemCap;
	}
}
