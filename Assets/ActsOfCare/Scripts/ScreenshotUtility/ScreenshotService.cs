using System;
using System.Collections;
using System.Collections.Generic;
using ActsOfCare.TingTingAdditions;
using ActsOfCare.Utility;
using TNG_Framework.TingTing.Services;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ActsOfCare.ScreenshotUtility
{
	public class ScreenshotService : IService
	{
		private const string LOCATION = "ScreenshotConfiguration";
		private ScreenshotConfiguration screenshotConfiguration;

		private Action<Texture2D> ScreenshotComplete;
		
		public string Id => "ScreenshotService";
		public bool IsInitialised { get; private set; }

		public void TakeScreenshot(Action<Texture2D> onScreenshotComplete, RectTransform referenceTransform = null)
		{
			var sceneRepository  = SceneRepository.Instance;
			if (sceneRepository == null)
			{
				Debug.LogError($"[ScreenshotService] SceneRepository not found");
				return;
			}
			if (!sceneRepository.TryGetSceneAccessDirectoryEntry(out CoroutineRunner coroutineRunner))
			{
				Debug.LogError($"[ScreenshotService] CoroutineRunner not found");
				return;
			}
			ScreenshotComplete += onScreenshotComplete;
			if (referenceTransform == null)
			{
				coroutineRunner.StartCoroutine(CaptureScreenshot());
			}
			else
			{
				coroutineRunner.StartCoroutine(CaptureScreenshot(referenceTransform));
			}
		}

		private IEnumerator CaptureScreenshot(RectTransform rectTransform)
		{
			yield return new WaitForEndOfFrame();

			// Capture full screen
			Texture2D fullTex = ScreenCapture.CaptureScreenshotAsTexture();
			var referenceRect = IntRect.From(rectTransform.ToTexturePixelRect(fullTex));
			CaptureScreenshotWithDimensions(fullTex,  referenceRect.X, 
				referenceRect.Y, 
				referenceRect.Width, 
				referenceRect.Height);
		}
		
		private IEnumerator CaptureScreenshot()
		{
			yield return new WaitForEndOfFrame();

			// Capture full screen
			Texture2D fullTex = ScreenCapture.CaptureScreenshotAsTexture();
			int x = screenshotConfiguration.X;
			int y = screenshotConfiguration.Y;
			int width = Math.Min(screenshotConfiguration.Width, fullTex.width);
			int height = Math.Min(screenshotConfiguration.Height, fullTex.height);
			CaptureScreenshotWithDimensions(fullTex,  x, y, width, height);
		}

		private void CaptureScreenshotWithDimensions(Texture2D fullTexture, int x, int y, int width, int height)
		{
			// Crop
			var croppedTex = new Texture2D(width, height, TextureFormat.RGB24, false);
			croppedTex.SetPixels(fullTexture.GetPixels(x, y, width, height));
			croppedTex.Apply();

			Object.Destroy(fullTexture);
			ScreenshotComplete?.Invoke(croppedTex);
		}
		
		public Type GetService()
		{
			return GetType();
		}
		public Object GetObject()
		{
			throw new NotImplementedException();
		}
		public void OnInit()
		{
			IsInitialised = true;
			var resourceRequest = Resources.LoadAsync<ScreenshotConfiguration>(LOCATION);
			resourceRequest.completed += operation =>
			{
				screenshotConfiguration = resourceRequest.asset as ScreenshotConfiguration;
				IsInitialised = true;
			};
		}
		public void OnRegister()
		{
			
		}
		
		public void OnDeregister()
		{
		}
	}
}