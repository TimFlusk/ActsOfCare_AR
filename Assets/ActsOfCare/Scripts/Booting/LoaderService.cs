using System;
using TNG_Framework.TingTing.Services;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ActsOfCare.Booting
{
	public class LoaderService : IService
	{
		private const string PRIMARY_LOADER_LOCATION = "PrimaryLoader";
		
		public string Id => "LoaderService";
		public bool IsInitialised { get; private set; }
		
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
			var primaryLoader = Resources.Load<PrimaryLoader>(PRIMARY_LOADER_LOCATION);
			primaryLoader.LoadElements();
			IsInitialised = true;
		}
		
		public void OnRegister()
		{
			
		}
		public void OnDeregister()
		{
			
		}
	}
}