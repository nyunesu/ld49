using ADEUtility;
using UnityEngine;

public class GhostTotem : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private void Update()
	{
		transform.position = Helper.GetMouseWorldPosition();
	}

	public void Display(TotemInfo totemInfo)
	{
		spriteRenderer.sprite = totemInfo.Sprite;
	}
}
