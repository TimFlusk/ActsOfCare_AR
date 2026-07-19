using System.IO;
using UnityEngine;
namespace ActsOfCare.Utility
{
	public static class Texture2DUtility
	{
		public static void SaveToPNG(this Texture2D texture, string fileName)
		{
			byte[] bytes = texture.EncodeToPNG();
			string path = Path.Combine(Application.persistentDataPath, fileName);
			File.WriteAllBytes(path, bytes);
			Debug.Log($"Texture saved to: {path}");
		}
	}
}