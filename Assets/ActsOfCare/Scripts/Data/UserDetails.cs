using System;
using System.IO;
using UnityEngine;
namespace ActsOfCare.Data
{
	[Serializable]
	public class UserDetails
	{
		public string Email { get; set; }
		public bool Consent { get; set; }
		public string SaveLocation => absoluteSaveLocation;
		public string FileName { get; private set; }
		
		private readonly string absoluteSaveLocation;
		public string AbsoluteSaveLocation => absoluteSaveLocation + "/" + FileName;

		public UserDetails(string imageSaveLocationDirectory)
		{
			imageSaveLocationDirectory = string.Empty;
			absoluteSaveLocation = Path.Join(Application.persistentDataPath, imageSaveLocationDirectory);
		}

		public void AssignFileName()
		{
			FileName = Guid.NewGuid() + ".png";
		}

		public static UserDetails CreateNoConsentUserDetails()
		{
			return new UserDetails(string.Empty)
			{
				Consent = false
			};
		}
		
		public static UserDetails Default()
		{
			return new UserDetails(string.Empty)
			{
				Consent = true,
				Email = "info@twonamegames.com",
				FileName = Guid.NewGuid() + "_default.png"
			};
		}
	}
}