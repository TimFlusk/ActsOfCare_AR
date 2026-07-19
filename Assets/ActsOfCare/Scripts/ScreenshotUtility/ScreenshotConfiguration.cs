using UnityEngine;
namespace ActsOfCare.ScreenshotUtility
{
	[CreateAssetMenu(fileName = "ScreenshotConfiguration", menuName = "Acts Of Care/Screenshot Configuration", order = 0)]
	public class ScreenshotConfiguration : ScriptableObject
	{
		[field: SerializeField]
		public int Width { get; private set; }
		[field: SerializeField]
		public int Height { get; private set; }
		
		[field: SerializeField]
		public int X { get; private set; }
		[field: SerializeField]
		public int Y { get; private set; }
	}
}