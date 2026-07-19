using System.Collections.Generic;
using UnityEngine;
namespace ActsOfCare.Booting
{
	[CreateAssetMenu(fileName = "PrimaryLoader", menuName = "Acts Of Care/Primary Loader", order = 0)]
	public class PrimaryLoader : ScriptableObject
	{
		[SerializeField]
		private List<MonoBehaviour> primaryElements;

		public void LoadElements()
		{
			foreach (var element in primaryElements)
			{
				Instantiate(element);
			}
		}
	}
}