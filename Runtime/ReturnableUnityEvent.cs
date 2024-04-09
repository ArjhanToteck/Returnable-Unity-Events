using UnityEngine;
using System;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEvent<T> : MonoBehaviour
	{
		public UnityEngine.Object targetObject;

		public Func<T> eventFunction;
	}
}