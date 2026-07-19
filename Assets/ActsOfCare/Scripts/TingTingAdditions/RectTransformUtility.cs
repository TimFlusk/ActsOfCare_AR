using UnityEngine;
namespace ActsOfCare.TingTingAdditions
{
	public static class RectTransformExtensions
	{
		public static Rect ToTexturePixelRect(this RectTransform rectTransform, Texture2D texture)
		{
			var corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);

			var bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
			var topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

			float texWidth  = texture.width;
			float texHeight = texture.height;

			int x = Mathf.RoundToInt((bottomLeft.x / Screen.width)  * texWidth);
			int y = Mathf.RoundToInt((bottomLeft.y / Screen.height) * texHeight);
			int width  = Mathf.RoundToInt(((topRight.x - bottomLeft.x) / Screen.width)  * texWidth);
			int height = Mathf.RoundToInt(((topRight.y - bottomLeft.y) / Screen.height) * texHeight);

			x = Mathf.Clamp(x, 0, Mathf.FloorToInt(texWidth));
			y = Mathf.Clamp(y, 0, Mathf.FloorToInt(texHeight));
			width  = Mathf.Clamp(width,  0, Mathf.FloorToInt(texWidth)  - x);
			height = Mathf.Clamp(height, 0, Mathf.FloorToInt(texHeight) - y);

			return new Rect(x, y, width, height);
		}
	}
}