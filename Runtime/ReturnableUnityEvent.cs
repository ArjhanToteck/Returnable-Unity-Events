using UnityEngine;
using System;

namespace ReturnableUnityEvents
{
	[Serializable]
	public class ReturnableUnityEvent<T> : MonoBehaviour
	{
		public Func<T> eventFunction;


	}
}