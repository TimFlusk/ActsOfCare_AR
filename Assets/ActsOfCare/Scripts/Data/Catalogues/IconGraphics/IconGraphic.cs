using System;
using UnityEditor;
using UnityEngine;
namespace ActsOfCare.Data.Catalogues.IconGraphics
{
	[Serializable]
	public class IconGraphic
	{
		[field: SerializeField]
		public string Id { get; private set; }
       
		[field: SerializeField]
		public Sprite Image { get; private set; }


#if UNITY_EDITOR
		public void SetId()
		{
			Id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Image));
		}
#endif
	}
}