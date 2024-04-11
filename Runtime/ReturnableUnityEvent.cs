using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEvent<T>
	{
		public UnityEngine.Object targetObject;
		public string methodName;

		[SerializeField]
		public List<object> parameters = new() { "" };

		[SerializeField]
		private string returnTypeName = typeof(T).FullName;

		public T Invoke()
		{
			// get type and method info
			Type targetType = targetObject.GetType();
			MethodInfo methodInfo = targetType.GetMethod(methodName);

			// method not found
			if (methodInfo == null)
			{
				Debug.LogError("Attempted to invoke an invalid method.");
				return default;
			}

			// static methods
			if (methodInfo.IsStatic)
			{
				return (T)methodInfo.Invoke(null, null);
			}

			// no target object
			if (targetObject == null)
			{
				Debug.LogError("Attempted to invoke an instance method without a target object.");
				return default;
			}

			// TODO: add parameter support
			return (T)methodInfo.Invoke(targetObject, null);
		}
	}
}