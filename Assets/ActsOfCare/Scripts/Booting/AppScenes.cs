namespace ActsOfCare.Booting
{
	public static class AppScenes
	{
		public const string BOOT_SCENE = "Boot";
#if UNITY_ANDROID
		public const string POST_BOOT_SCENE = "SignUp";
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		public const string POST_BOOT_SCENE = "PosterWall";
#endif
		
		private static string[] sceneNames = new[]
		{
			"Boot",
			"SignUp",
			"PosterMaking",
			"PosterWall"
		};
		
		public static bool IsValidScene(string sceneName)
		{
			foreach (var name in sceneNames)
			{
				if (name == sceneName)
				{
					return true;
				}
			}
			return false;
		}
	}
}