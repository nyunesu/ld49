using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TotemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private TotemInfo totemInfo;

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
		GameManager.GetInstance().Select(totemInfo);
	}

	private void Refresh()
	{
		button.interactable = GameManager.GetInstance().Gold >= totemInfo.Cost;
	}
}
