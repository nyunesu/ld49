using System.Collections.Generic;
using System.Linq;
using ADEUtility;
using UnityEngine;
using UnityEngine.EventSystems;

public class Totem : MonoBehaviour
{
	public TotemInfo Info;
	private int points;
	private bool onDrag;
	private Vector3 startDragPosition;

	private void Update()
	{
		if (onDrag)
		{
			HandleDrag();
		}
	}

	private void OnMouseDown()
	{
		startDragPosition = transform.position;
		onDrag = true;
	}

	public void Setup(TotemInfo totemInfo)
	{
		Info = totemInfo;
	}

	public void Upgrade(int pointIncrement = 1)
	{
		points += pointIncrement;

		if (Info.Upgrade != null && points >= Info.Upgrade.Cost) { }
	}

	private void HandleDrag()
	{
		transform.position = Helper.GetMouseWorldPosition();

		if (!Input.GetMouseButtonUp(0))
			return;

		onDrag = false;
		Place();
	}

	private void Place()
	{
		Helper.GetAllObjectsInCircleRadius(
			transform.position,
			XGHGameManager.GetInstance().UpgradeDistance,
			out List<Totem> totemsInUpgradeRange
		);

		totemsInUpgradeRange = totemsInUpgradeRange.Where(
				x => Vector2.Distance(x.transform.position, transform.position)
					< XGHGameManager.GetInstance().UpgradeDistance
			)
			.ToList();

		totemsInUpgradeRange.Remove(this);

		if (totemsInUpgradeRange.Count > 0 || EventSystem.current.IsPointerOverGameObject())
			transform.position = startDragPosition;
	}

	private void Die()
	{
		XGHGameManager.GetInstance().TotemCount--;
		Destroy(gameObject);
	}
}
