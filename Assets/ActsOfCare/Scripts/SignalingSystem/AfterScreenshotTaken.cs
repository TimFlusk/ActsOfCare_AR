using UnityEngine;
namespace ActsOfCare.SignalingSystem
{
	public struct AfterScreenshotTaken : ISignal
	{
		public Texture2D PortraitTexture;
	}
}