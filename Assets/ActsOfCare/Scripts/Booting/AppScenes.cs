namespace ActsOfCare.Booting
{
	public static class AppScenes
	{
		public const string BOOT_SCENE = "Boot";
		public const string POST_BOOT_SCENE = "ActsOfCarePortrait";
		
		private static string[] sceneNames = new[]
		{
			"Boot",
			"ActsOfCarePortrait",
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