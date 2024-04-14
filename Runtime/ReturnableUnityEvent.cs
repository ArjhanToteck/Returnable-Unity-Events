using UnityEngine;
using System;
using System.Reflection;
using ReturnableUnityEvents.Editor;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEvent<T>
	{

		public UnityEngine.Object targetObject;

		// TODO: add overload support, this is not gonna work
		public string methodName;

		[SerializeField]
		private string parametersJSON = null;

		[SerializeField]
		private string returnTypeName = typeof(T).FullName;

		public T Invoke()
		{
			// get type and method info
			Type targetType = targetObject.GetType();
			MethodInfo methodInfo = targetType.GetMethod(methodName);

			object[] parameters = null;

			if (parametersJSON != "")
			{
				parameters = ParameterSerializer.DeserializeParameters(parametersJSON).ToArray();
			}

			// method not found
			if (methodInfo == null)
			{
				Debug.LogError("Attempted to invoke an invalid method.");
				return default;
			}

			// static methods
			if (methodInfo.IsStatic)
			{
				return (T)methodInfo.Invoke(null, parameters);
			}

			// no target object
			if (targetObject == null)
			{
				Debug.LogError("Attempted to invoke an instance method without a target object.");
				return default;
			}

			return (T)methodInfo.Invoke(targetObject, parameters);
		}
	}
}