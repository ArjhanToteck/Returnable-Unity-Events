using UnityEngine;
using System;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEvent<T>
	{
		public UnityEngine.Object targetObject;
		public string methodName;

		public T Invoke()
		{
			return default;
		}
	}
}