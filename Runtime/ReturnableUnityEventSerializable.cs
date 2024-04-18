using UnityEngine;
using System;
using System.Reflection;
using ReturnableUnityEvents.Editor;
using Object = UnityEngine.Object;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEventSerializable : ScriptableObject
	{
		public Object targetObject;

		// TODO: add overload support, this is not gonna work
		public string methodName;

		[SerializeField]
		private string parametersJSON = null;

		[SerializeField]
		private string returnTypeName = null;
	}
}