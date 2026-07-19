using System;
using System.Collections;
using UnityEngine;
namespace ActsOfCare.Utility
{
	public class CoroutineRunner : MonoBehaviour
	{
		public void StartCoroutine(Func<IEnumerator>routine)
		{
			StartCoroutine(routine());
		}
	}
}