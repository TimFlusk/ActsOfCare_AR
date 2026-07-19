using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActsOfCare.Booting;
using ActsOfCare.ScreenshotUtility;
using ActsOfCare.SignalingSystem;
using ActsOfCare.Uploading;
using TNG_Framework.TingTing.Services;
using UnityEngine;
namespace ActsOfCare.TingTingAdditions
{
	public static class ServiceLocator
	{
		public static bool InitialServicesRegistered { get; private set; }
		
		private static ServiceCollection services = new();
		
		private static Dictionary<Type, IService> serviceCatalogue = new();

		private static Action ServicesReady;
		
		private static TaskCompletionSource<bool> serviceCompletedInitialisation;
		

		public static bool TryGetService<T>(out T service) where T : IService
		{
			if (serviceCatalogue.ContainsKey(typeof(T)))
			{
				service = (T)serviceCatalogue[typeof(T)];
				return true;
			}
			service = default;
			return false;
		}
		
		public static void Initialise()
		{
			services
				.AddService<UploadCoordinator>()
				.AddService<UserManager>()
				//.AddService<LoaderService>()
				.AddService<SignalingService>()
				.AddService<ScreenshotService>();

			PollServicesInitialised();
		}

		private static async void PollServicesInitialised()
		{
			await PollServicesInitialisedAsync();
			InitialServicesRegistered = true;
			Debug.Log("Service initialised");
			foreach (var service in services)
			{
				service.OnRegister();
			}
			ServicesReady?.Invoke();
		}

		public static void SubscribeToServicesReady(Action onReady)
		{
			ServicesReady += onReady;
		}

		public static void DeleteSubscriptions()
		{
			ServicesReady = null;
		}
		
		private static async Task PollServicesInitialisedAsync(CancellationToken cancellationToken = default)
		{
			serviceCompletedInitialisation = new TaskCompletionSource<bool>(false);
			await Task.Run(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					foreach (var service in services)
					{
						if (service.IsInitialised)
						{
							serviceCatalogue[service.GetType()] = service;
						}
					}

					if (serviceCatalogue.Count == services.Count)
					{
						serviceCompletedInitialisation.SetResult(true);
						return;
					}

					await Task.Delay(100, cancellationToken);
				}
				serviceCompletedInitialisation.TrySetCanceled(cancellationToken);

			}, cancellationToken);
		}
	}
}