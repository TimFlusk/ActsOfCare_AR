using ActsOfCare.SignalingSystem;
namespace ActsOfCare.BodyTracking
{
	public struct BodyLost : ISignal
	{
		public static BodyLost Create()
		{
			return new BodyLost();
		}
	}
}