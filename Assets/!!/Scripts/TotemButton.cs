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
		GameManager.GetInstance().SetTooltip(true, totemInfo.GetShopDescription());
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.GetInstance().SetTooltip(false);
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
		GameManager.GetInstance().OnHudChanged += Refresh;
	}

	private void OnDisable()
	{
		GameManager.GetInstance().OnHudChanged -= Refresh;
	}

	private void Purchase()
	{
		GameManager.GetInstance().Purchase(totemInfo);
		Destroy(gameObject);
	}

	private void Refresh()
	{
		var gameManager = GameManager.GetInstance();
		button.interactable = GameManager.GetInstance().Gold >= totemInfo.Cost && gameManager.TotemCount < gameManager.TotemCap;
	}
}
