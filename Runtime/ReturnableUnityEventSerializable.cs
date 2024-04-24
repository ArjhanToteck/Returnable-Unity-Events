using UnityEngine;
using System;
using System.Reflection;
using ReturnableUnityEvents.Editor;
using Object = UnityEngine.Object;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEventSerializableWrapper : ScriptableObject
	{
		public string test = "asd";
		public ReturnableUnityEvent<object> returnableUnityEvent;
	}
}