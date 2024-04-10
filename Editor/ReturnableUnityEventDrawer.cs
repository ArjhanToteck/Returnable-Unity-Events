using UnityEngine;
using UnityEditor;
using System.Reflection.Emit;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace ReturnableUnityEvents
{
	[CustomPropertyDrawer(typeof(ReturnableUnityEvent<>))]
	public class ReturnableUnityEventDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// get properties
			SerializedProperty targetObjectProperty = property.FindPropertyRelative("targetObject");
			SerializedProperty methodNameProperty = property.FindPropertyRelative("methodName");

			// draw label and configure position
			EditorGUI.BeginProperty(position, label, property);
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
			EditorGUI.ObjectField(pos1, targetObjectProperty, typeof(UnityEngine.Object), new GUIContent(""));

			// check if no object selected
			if (targetObjectProperty.objectReferenceValue == null)
			{
				// function selection (disabled by default)
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Popup(pos2, 0, new GUIContent[] {
					new GUIContent("No Function")
				});
				EditorGUI.EndDisabledGroup();

				// remove method name
				methodNameProperty.stringValue = null;
			}
			else
			{
				// display no function or actual method name
				string displayedMethodName = methodNameProperty.stringValue != null && methodNameProperty.stringValue != "" ? methodNameProperty.stringValue : "No Function";

				// draw enabled function popup (technically a button)
				var functionPopupPressed = GUI.Button(pos2, new GUIContent(displayedMethodName), EditorStyles.popup);

				if (functionPopupPressed)
				{
					// create search window
					FunctionSearchWindow searchWindow = ScriptableObject.CreateInstance<FunctionSearchWindow>();

					// get object type
					Type objectType = targetObjectProperty.GetType();

					// get methods
					MethodInfo[] instanceMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
					MethodInfo[] staticMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Static);

					// get property getters and setters

					// instance
					List<PropertyInfo> instanceProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
					MethodInfo[] instancePropertyGetters = instanceProperties.Select(x => x.GetMethod).ToArray();
					MethodInfo[] instancePropertySetters = instanceProperties.Select(x => x.SetMethod).ToArray();

					// static
					List<PropertyInfo> staticProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Static).ToList();
					MethodInfo[] staticPropertyGetters = staticProperties.Select(x => x.GetMethod).ToArray();
					MethodInfo[] staticPropertySetters = staticProperties.Select(x => x.SetMethod).ToArray();

					// exclude property methods from regular method lists
					instanceMethods = instanceMethods.Except(instancePropertyGetters).Except(instancePropertySetters).ToArray();
					staticMethods = staticMethods.Except(staticPropertyGetters).Except(staticPropertySetters).ToArray();

					// TODO: probably want to add field "getters," though setters wouldn't make sense
					// get fields
					// List<FieldInfo> instanceFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
					// List<FieldInfo> staticFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Static).ToList();

					// add methods to search window

					searchWindow.searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Instance"), 1));
					searchWindow.AddGroup("Methods", instanceMethods, 2);
					searchWindow.searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Properties"), 2));
					searchWindow.AddGroup("Getters", instancePropertyGetters, 3);
					searchWindow.AddGroup("Setters", instancePropertySetters, 3);

					searchWindow.searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Static"), 1));
					searchWindow.AddGroup("Methods", staticMethods, 2);
					searchWindow.searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Properties"), 2));
					searchWindow.AddGroup("Getters", staticPropertyGetters, 3);
					searchWindow.AddGroup("Setters", staticPropertySetters, 3);

					// set callback
					searchWindow.onSelectEntryCallback = new Action<SearchTreeEntry>((SearchTreeEntry) =>
					{
						// set method name property to selection
						methodNameProperty.stringValue = (string)SearchTreeEntry.userData;
						property.serializedObject.ApplyModifiedProperties();
					});

					// open search window
					SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchWindow);
				}
			}

			// reset indent settings
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
