using System;
using System.IO;
using ActsOfCare.Data;
using ActsOfCare.TingTingAdditions;
using ActsOfCare.Uploading;
using ActsOfCare.Utility;
using TNG_Framework.TingTing.Services;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ActsOfCare
{
	public class UserManager : IService
	{
		public const PayloadContext PAYLOAD_CONTEXT = PayloadContext.Poster;
		public UserDetails CurrentUserDetails { get; private set; }

		private const string FILE_NAME = "userdetails.json";
		private readonly string absolutePath = Path.Join(Application.persistentDataPath, FILE_NAME);

		private UserDetailsCollection userDetailsCollection = new();

		private Texture2D screenshot = null;

		private UploadCoordinator uploadCoordinator;


		public string Id => "UserManager";

		public bool IsInitialised => isInitialized;
		private bool isInitialized = false;

		public void SetUserDetails(UserDetails userDetails)
		{
			CurrentUserDetails = userDetails;
		}
		
		public void SetScreenshot(Texture2D screenshot)
		{
			this.screenshot = screenshot;
		}

		public void SaveUserDetails()
		{
			userDetailsCollection.Add(CurrentUserDetails);
			using var streamWriter = new StreamWriter(absolutePath, false);
			var content =  JsonUtility.ToJson(userDetailsCollection);
			streamWriter.Write(content);
			var imageFilePath = Path.Join(Application.persistentDataPath, FILE_NAME);
			screenshot.SaveToPNG(imageFilePath);
		}

		public void UploadUserDetailsAndImage()
		{
			uploadCoordinator.TriggerUpload(CurrentUserDetails, PAYLOAD_CONTEXT);
		}
		
		private void GatherLocalUserDetails()
		{
			if (!File.Exists(absolutePath))
			{
				return;
			}
			using var streamReader = new StreamReader(absolutePath);
			var content = streamReader.ReadToEnd();
			userDetailsCollection = JsonUtility.FromJson<UserDetailsCollection>(content);
		}
		
		public Type GetService()
		{
			throw new NotImplementedException();
		}
		public Object GetObject()
		{
			throw new NotImplementedException();
		}
		public void OnInit()
		{
			GatherLocalUserDetails();
			isInitialized = true;
			Debug.Log("User Manager Initialized");
		}
		
		public void OnRegister()
		{
			if (!ServiceLocator.TryGetService(out uploadCoordinator))
			{
				Debug.LogError("Unable to find UploadCoordinator");
			}
			Debug.Log("User Manager Registered");
		}
		public void OnDeregister()
		{
			
		}
	}
}