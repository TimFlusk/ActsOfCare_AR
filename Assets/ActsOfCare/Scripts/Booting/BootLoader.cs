using ActsOfCare.TingTingAdditions;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ActsOfCare.Booting
{
	public static class BootLoader
	{
		private static string CurrentSceneName;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Boot()
		{
			var sceneName = SceneManager.GetActiveScene().name;
#if UNITY_EDITOR
			CurrentSceneName = AppScenes.IsValidScene(sceneName) ? sceneName : AppScenes.POST_BOOT_SCENE;
#else
			CurrentSceneName = AppScenes.POST_BOOT_SCENE;
#endif
			Debug.Log("POST BOOT SCENE: " + CurrentSceneName);
#if UNITY_EDITOR
			SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
			SceneManager.LoadScene(AppScenes.BOOT_SCENE);
#else
			InitialiseServiceLocator();
#endif
		}
		private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (scene.name == AppScenes.BOOT_SCENE)
			{
				InitialiseServiceLocator();
			}
		}

		private static void InitialiseServiceLocator()
		{
			if (!ServiceLocator.InitialServicesRegistered)
			{
				ServiceLocator.SubscribeToServicesReady(OnServicesReady);
				ServiceLocator.Initialise();
			}
		}
		
		private static void OnServicesReady()
		{
			SceneManager.LoadScene(CurrentSceneName);
		}
	}
}