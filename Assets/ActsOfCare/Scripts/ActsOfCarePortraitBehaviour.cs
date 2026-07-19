using TNG_Framework.TingTing.Gameplay;
namespace ActsOfCare
{
	public class ActsOfCarePortraitBehaviour : TingTingGameBehaviour
	{
		protected SceneRepository sceneRepository;
		
		protected virtual void Start()
		{
			sceneRepository = SceneRepository.Instance;
		}
	}
}