using System.Collections.Generic;
using UnityEngine;
namespace ActsOfCare.Data.Catalogues.IconGraphics
{
	[CreateAssetMenu(fileName = "IconGraphicCatalogue", menuName = "Acts Of Care/Icon Graphic Catalogue", order = 0)]
	public class IconGraphicCatalogue : ScriptableObject
	{
		[SerializeField]
		private List<IconGraphic> icons;
		public List<IconGraphic> Icons => icons;

#if UNITY_EDITOR
		[SerializeField]
		private string sourceFolderPath = "Assets/";
#endif

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (icons.Count == 0)
			{
				return;
			}
			foreach (var iconGraphic in icons)
			{
				iconGraphic.SetId();
			}
		}
#endif
	}
}