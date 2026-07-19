using System;
using ActsOfCare.BodyTracking;
using ActsOfCare.SignalingSystem;
using ActsOfCare.TingTingAdditions;
using ActsOfCare.Uploading;
using TNG_Framework.TingTing.Chrono;
using UnityEngine;
namespace ActsOfCare.ScreenshotUtility
{
	public class AutomaticScreenshot : ActsOfCarePortraitBehaviour
	{
		// Every 30 seconds when possible
		[SerializeField]
		private float screenshotTime = 30;

		[SerializeField]
		private bool logTimer;
		
		private Timer timer;

		private SignalingService signalingService;
		private ScreenshotService screenshotService;
		private UploadCoordinator uploadCoordinator;

		private bool isBodyDetected;
		
		protected override void Start()
		{
			base.Start();
			if (!ServiceLocator.TryGetService(out signalingService))
			{
				Debug.LogError($"{nameof(SignalingService)} could not be found.");
			}
			if (!ServiceLocator.TryGetService(out screenshotService))
			{
				Debug.LogError($"{nameof(ScreenshotService)} could not be found.");
			}
			if (!ServiceLocator.TryGetService(out uploadCoordinator))
			{
				Debug.LogError($"{nameof(UploadCoordinator)} could not be found.");
			}
			signalingService?.SubscribeToSignal<BodyDetected>(OnBodyDetected);
			signalingService?.SubscribeToSignal<BodyLost>(OnBodyLost);
		}

		private void Update()
		{
			timer?.Tick(Time.deltaTime);
		}

		private void OnBodyDetected(BodyDetected obj)
		{
			timer = new Timer(screenshotTime, OnCountdownToScreenshotComplete, OnCountDownTick);
			isBodyDetected = true;
		}
		
		private void OnBodyLost(BodyLost obj)
		{
			timer = null;
			isBodyDetected = false;
		}
		
		private void OnCountDownTick(float currentTime, float normalisedTime)
		{
			if (!logTimer)
			{
				return;
			}
			Debug.Log($"<color=green>Screenshot Countdown Time: {currentTime}</color>");
		}
		
		private void OnCountdownToScreenshotComplete()
		{
			Debug.Log("<color=green>TAKE SCREENSHOT</color>");
			screenshotService.TakeScreenshot(OnScreenShotComplete);
			timer = new Timer(screenshotTime, OnCountdownToScreenshotComplete, OnCountDownTick);
			
		}
		private void OnScreenShotComplete(Texture2D screenshot)
		{
			uploadCoordinator.SaveScreenshot(screenshot);
			
			uploadCoordinator.TriggerUpload(UserManager.PAYLOAD_CONTEXT);
			
			signalingService.ActivateSignal(new AfterScreenshotTaken()
			{
				PortraitTexture = screenshot,
			});
			Debug.Log("<color=green>Screenshot Complete</color>");
		}
	}
}