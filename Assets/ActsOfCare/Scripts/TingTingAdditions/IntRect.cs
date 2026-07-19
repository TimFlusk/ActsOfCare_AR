using UnityEngine;
namespace ActsOfCare.TingTingAdditions
{
	public class IntRect
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		
		public IntRect(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public static IntRect From(Rect rect)
		{
			int x = (int) rect.x;
			int y = (int) rect.y;
			int width = (int) rect.width;
			int height = (int)rect.height;
			return new IntRect(x, y, width, height);
		}
	}
}