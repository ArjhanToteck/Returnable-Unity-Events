using UnityEngine;
using UnityEditor;
using System.Reflection.Emit;
using Codice.CM.Client.Gui;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace ReturnableUnityEvents
{
	[CustomPropertyDrawer(typeof(ReturnableUnityEvent<>))]
	public class ReturnableUnityEventDrawer : PropertyDrawer
	{
		private UnityEngine.Object targetObject = null;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// draw label and configure position
			Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);
			position = EditorGUI.PrefixLabel(labelPosition, EditorGUIUtility.GetControlID(FocusType.Passive), label);

			// configure indent
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// get width and offset
			float widthSize = position.width / 2;
			float offsetSize = 2;

			// get positions for inputs
			Rect pos1 = new Rect(position.x, position.y, widthSize - offsetSize, position.height);
			Rect pos2 = new Rect(position.x + offsetSize + widthSize, position.y, widthSize - offsetSize, position.height);

			// object field
			var newTargetObject = EditorGUI.ObjectField(pos1, targetObject, typeof(UnityEngine.Object), true);

			// check if new object is selected
			if (newTargetObject != targetObject)
			{
				targetObject = newTargetObject;
			}

			// check if no object selected
			if (targetObject == null)
			{
				// TODO: implement a search window instead of a popup: https://www.youtube.com/watch?v=0HHeIUGsuW8

				// function selection (disabled by default)
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Popup(pos2, 0, new GUIContent[] {
					new GUIContent("No Function")
				});
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				// list functions

				// get object type
				Type objectType = targetObject.GetType();

				// get static and non-static methods, fields, and properties
				MethodInfo[] staticMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Static);
				MethodInfo[] instanceMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

				PropertyInfo[] staticProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Static);
				PropertyInfo[] instanceProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

				FieldInfo[] staticFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance);
				FieldInfo[] instanceFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance);


			}

			// reset indent settings
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
