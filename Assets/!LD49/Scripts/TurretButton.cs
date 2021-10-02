using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TurretButton : MonoBehaviour
{
	[SerializeField]
	private TurretData turretData;

	[SerializeField]
	private TMP_Text goldCost;

	[SerializeField]
	private Image buttonImage;

	private Button button;

	private void Awake()
	{
		TryGetComponent(out button);
		button.onClick.AddListener(() => TurretSpawner.GetInstance().SelectTurret(turretData));
	}

	private void Start()
	{
		if (goldCost)
			goldCost.SetText(turretData.Cost.ToString());

		if (buttonImage)
			buttonImage.sprite = turretData.Sprite;
	}
}
