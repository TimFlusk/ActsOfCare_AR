using UnityEngine;
namespace ActsOfCare.Utility
{
	public static class CanvasGroupUtility
	{
		public static void SwitchOn(this CanvasGroup canvasGroup, float alpha = 1, bool blocksRaycasts = true, bool isInteractable = true)
		{
			canvasGroup.alpha = alpha;
			canvasGroup.blocksRaycasts =  true;
			canvasGroup.interactable = isInteractable;

		}
		
		public static void SwitchOff(this CanvasGroup canvasGroup)
		{
			canvasGroup.alpha = 0;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
		}
	}
}