using UnityEngine;

public class ColorHelper
{
	public static Color Green
	{
		get => GetColorFromHex("#8bbf40");
	}

	public static Color Red
	{
		get => GetColorFromHex("#e91d39");
	}

	public static Color Blue
	{
		get => GetColorFromHex("#019bd6");
	}

	public static Color Yellow
	{
		get => GetColorFromHex("#facf00");
	}

	public static Color Purple
	{
		get => GetColorFromHex("#8e559e");
	}

	public static Color Orange
	{
		get => GetColorFromHex("#f07021");
	}

	public static Color Turquoise
	{
		get => GetColorFromHex("#017866");
	}

	public static Color LightGray
	{
		get => GetColorFromHex("#606060");
	}

	public static Color LighterGray
	{
		get => GetColorFromHex("#b0a89f");
	}

	public static Color WhiteGray
	{
		get => GetColorFromHex("#dadada");
	}

	public static Color DarkGray
	{
		get => GetColorFromHex("#303030");
	}

	public static Color DarkerGray
	{
		get => GetColorFromHex("#272727");
	}

	public static Color White
	{
		get => GetColorFromHex("#FFFFFF");
	}

	public static Color Black
	{
		get => GetColorFromHex("#000000");
	}

	public static Color GetColorFromHex(string hexColor)
	{
		return ColorUtility.TryParseHtmlString(hexColor, out Color color) ? color : Color.magenta;
	}
}