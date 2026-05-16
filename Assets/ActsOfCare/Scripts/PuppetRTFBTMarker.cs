// ── Marker component ───────────────────────────────────────────────────────

using UnityEngine;

namespace ActsOfCare
{
	/// <summary>
	/// Placeholder that documents RTFBT wiring instructions on the root object.
	/// Replace with the actual PoseTracking component from the RTFBT asset.
	/// Fields here mirror the RTFBT PoseTracking inspector fields documented
	/// in the README (In Place, Match Scale).
	/// </summary>
	public class PuppetRTFBTMarker : MonoBehaviour
	{
		[Header("RTFBT PoseTracking Settings")]
		[Tooltip("Keep avatar position fixed — prevents Z-drift into the scene.")]
		public bool InPlace = true;

		[Tooltip("Auto-scale avatar based on camera distance. Disable for 2D.")]
		public bool MatchScale = false;

		[Header("Wiring Instructions")]
		[TextArea(4, 8)]
		public string Note;

		[Header("Bone Transform References")]
		[Tooltip("Assign child GameObjects to the matching bone slots on PoseTracking.")]
		public Transform Head;
		public Transform Torso;
		public Transform LeftShoulder;
		public Transform RightShoulder;
		public Transform LeftArm;
		public Transform RightArm;
		public Transform LeftThigh;
		public Transform RightThigh;
		public Transform LeftFoot;
		public Transform RightFoot;
	}
}