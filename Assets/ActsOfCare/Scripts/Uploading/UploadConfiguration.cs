using UnityEngine;
using UnityEngine.Serialization;
namespace ActsOfCare.Uploading
{
	[CreateAssetMenu(fileName = "UploadConfiguration", menuName = "Acts Of Care/Upload Configuration", order = 0)]
	public class UploadConfiguration : ScriptableObject
	{
		[Header("Server")]
		[field: SerializeField] 
		public string ServerBaseUrl { get; private set; }

		[Tooltip("Per-request timeout in seconds.")]
		[field: SerializeField] 
		public int TimeoutSeconds { get; private set; }
		
		[Tooltip("Seconds between retry passes while the app is running.")]
		[field: SerializeField] 
		public float RetryIntervalSeconds { get; private set; }

		[field: SerializeField] 
		public int MaxRetries;
	}
}