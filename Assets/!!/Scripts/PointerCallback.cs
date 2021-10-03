using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Action enterCallback;
	private Action exitCallback;

	public void OnPointerEnter(PointerEventData eventData)
	{
		enterCallback();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		exitCallback();
	}

	public void SetCallback(Action enterCallback, Action exitCallback)
	{
		this.enterCallback = enterCallback;
		this.exitCallback = exitCallback;
	}
}
