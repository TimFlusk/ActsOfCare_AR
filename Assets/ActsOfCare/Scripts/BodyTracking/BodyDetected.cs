using ActsOfCare.SignalingSystem;
namespace ActsOfCare.BodyTracking
{
	public struct BodyDetected : ISignal
	{
		public static BodyDetected Create()
		{
			return new BodyDetected();
		}
	}
}