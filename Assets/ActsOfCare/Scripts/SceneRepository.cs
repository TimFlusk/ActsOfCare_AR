using System;
using System.Collections.Generic;
using TNG_Framework.TingTing.Patterns;
using UnityEngine;
namespace ActsOfCare
{
	public class SceneRepository : Singleton<SceneRepository>
	{
		[SerializeField]
		private List<MonoBehaviour> entries;

		[SerializeField]
		private List<Behaviour> behaviours;
		
		
		private Dictionary<Type, MonoBehaviour> sceneAccessDirectory;
		
		private Dictionary<Type, Behaviour> behavioursDirectory;
		
		public bool TryGetSceneAccessDirectoryEntry<T>(out T monoBehaviour) where T : MonoBehaviour
		{
			MonoBehaviour entry;
			Debug.Log("Type searching for " + typeof(T));
			if (sceneAccessDirectory.TryGetValue(typeof(T), out entry))
			{
				monoBehaviour = (T)entry;
				return true;
			}
			monoBehaviour = null;
			return false;
		}
		
		public bool TryGetSceneAccessDirectoryBehaviour<T>(out T behaviour) where T : Behaviour
		{
			Behaviour entry;
			Debug.Log("Type searching for " + typeof(T));
			if (behavioursDirectory.TryGetValue(typeof(T), out entry))
			{
				behaviour = (T)entry;
				return true;
			}
			behaviour = null;
			return false;
		}

		protected override void Awake()
		{
			base.Awake();
			PopulateSceneAccessDirectory();
			PopulateBehaviours();
		}
		private void PopulateBehaviours()
		{
			behavioursDirectory = new Dictionary<Type, Behaviour>();
			foreach (var behaviour in behaviours)
			{
				var type = behaviour.GetType();
				Debug.Log($"Type: {type}");
				if (behavioursDirectory.ContainsKey(type))
				{
					continue;
				}
				behavioursDirectory[type] = behaviour;
			}
		}

		private void PopulateSceneAccessDirectory()
		{
			sceneAccessDirectory = new Dictionary<Type, MonoBehaviour>();
			foreach (var monoBehaviour in entries)
			{
				var type = monoBehaviour.GetType();
				Debug.Log($"Type: {type}");
				if (sceneAccessDirectory.ContainsKey(type))
				{
					continue;
				}
				sceneAccessDirectory[type] = monoBehaviour;
			}
		}
	}
}