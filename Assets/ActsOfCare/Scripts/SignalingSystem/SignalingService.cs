using System;
using System.Collections.Generic;
using TNG_Framework.TingTing.Services;
using Object = UnityEngine.Object;
namespace ActsOfCare.SignalingSystem
{
	public class SignalingService : IService
	{
		private List<Type> signalDataEvents = new();

		private Dictionary<Type, Action<ISignal>> signalEventMapper = new();
		
		public void PrepareSignal<T>() where T : ISignal
		{
			if (!signalEventMapper.ContainsKey(typeof(T)))
			{
				signalDataEvents.Add(typeof(T));
				signalEventMapper[typeof(T)] = null;
			}
		}

		public void SubscribeToSignal<T>(Action<T> callback) where T : ISignal
		{
			Action<ISignal> wrapper = signal => callback((T)signal);
			if (signalEventMapper.ContainsKey(typeof(T)))
			{
				signalEventMapper[typeof(T)] += wrapper;
			}
			else
			{
				signalEventMapper[typeof(T)] = wrapper;
			}
		}

		public void ActivateSignal<T>(T signal) where T : ISignal
		{
			if (signalEventMapper.ContainsKey(typeof(T)))
			{
				signalEventMapper[typeof(T)]?.Invoke(signal);
			}
		}
		
		public string Id => "SignalService";
		public bool IsInitialised => isInitialised;
		private bool isInitialised = false;
		public Type GetService()
		{
			return typeof(SignalingService);
		}
		public Object GetObject()
		{
			throw new NotImplementedException();
		}
		public void OnInit()
		{
			isInitialised = true;
		}
		public void OnRegister()
		{
		}
		public void OnDeregister()
		{
		}
	}
}