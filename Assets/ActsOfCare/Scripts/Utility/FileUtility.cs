using System.IO;
using UnityEngine;
namespace ActsOfCare.Utility
{
	public static class FileUtility
	{
		public static void EnsureDirectoriesExist(string path)
		{
			var directory = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory); 
			}
		}
		
		public static void EnsurePathExists(string path)
		{
			var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var current = parts[0];

			for (int i = 1; i < parts.Length; i++)
			{
				current = Path.Combine(current, parts[i]);
				Debug.Log(current);
				// Skip the final segment if it looks like a file
				bool isFinalSegment = i == parts.Length - 1;
				bool isFile = isFinalSegment && Path.HasExtension(parts[i]);
        
				if (!isFile)
				{
					if (!Directory.Exists(current))
					{
						Directory.CreateDirectory(current);
						Debug.Log($"Created: {current}");
					}
				}
			}
		}
	}
}