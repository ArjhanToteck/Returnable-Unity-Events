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
			SerializedProperty parametersProperty = property.FindPropertyRelative("parameters");
			Type returnType = Type.GetType(property.FindPropertyRelative("returnTypeName").stringValue);

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
			UnityEngine.Object oldTargetObject = targetObjectProperty.objectReferenceValue;
			EditorGUI.ObjectField(pos1, targetObjectProperty, typeof(UnityEngine.Object), new GUIContent(""));
			bool targetObjectChanged = oldTargetObject != targetObjectProperty.objectReferenceValue;

			// check if no object selected or if the object changed
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
				// display  method name
				string displayedMethodName = methodNameProperty.stringValue;
				bool functionSelected = true;

				// reset method name if empty, null, or object just changed
				if (displayedMethodName == "" || displayedMethodName == null || targetObjectChanged)
				{
					displayedMethodName = "No Function";
					functionSelected = false;

					// remove method name in serialized object
					methodNameProperty.stringValue = null;
				}

				// draw enabled function popup (technically a button)
				var functionPopupPressed = GUI.Button(pos2, new GUIContent(displayedMethodName), EditorStyles.popup);

				if (functionPopupPressed)
				{
					// create search window
					FunctionSearchWindow searchWindow = ScriptableObject.CreateInstance<FunctionSearchWindow>();

					// get object type
					Type objectType = targetObjectProperty.objectReferenceValue.GetType();

					// get methods
					List<MethodInfo> instanceMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
					List<MethodInfo> staticMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList(); ;

					// get property getters and setters

					// instance
					List<PropertyInfo> instanceProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
					List<MethodInfo> instancePropertyGetters = instanceProperties.Select(x => x.GetMethod).ToList();
					List<MethodInfo> instancePropertySetters = instanceProperties.Select(x => x.SetMethod).ToList();

					// static
					List<PropertyInfo> staticProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Static).ToList();
					List<MethodInfo> staticPropertyGetters = staticProperties.Select(x => x.GetMethod).ToList();
					List<MethodInfo> staticPropertySetters = staticProperties.Select(x => x.SetMethod).ToList();

					// exclude property methods from regular method lists
					instanceMethods = instanceMethods.Except(instancePropertyGetters).Except(instancePropertySetters).ToList();
					staticMethods = staticMethods.Except(staticPropertyGetters).Except(staticPropertySetters).ToList();

					// TODO: probably want to add field "getters," though setters wouldn't make sense. could probably just have a bool in ReturnableUnityEvent that says we have a field instead of an actual method and then just get the method using its name in ReturnableUnityEvent.Invoke
					// get fields
					// List<FieldInfo> instanceFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
					// List<FieldInfo> staticFields = objectType.GetFields(BindingFlags.Public | BindingFlags.Static).ToList();

					// filter methods by type
					instanceMethods = FilterMethodInfosByType(instanceMethods, returnType);
					staticMethods = FilterMethodInfosByType(staticMethods, returnType);
					instancePropertyGetters = FilterMethodInfosByType(instancePropertyGetters, returnType);
					instancePropertySetters = FilterMethodInfosByType(instancePropertySetters, returnType);
					staticPropertyGetters = FilterMethodInfosByType(staticPropertyGetters, returnType);
					staticPropertySetters = FilterMethodInfosByType(staticPropertySetters, returnType);

					// TODO: don't add any groups that will be empty
					// add methods to search window
					searchWindow.searchTree.Add(new SearchTreeGroupEntry(new GUIContent(returnType.Name + " Functions")));
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
						MethodInfo function = (MethodInfo)SearchTreeEntry.userData;

						// set method name property to selection
						methodNameProperty.stringValue = function.Name;
						// clear parameters list to prepare for updating it
						parametersProperty.ClearArray();

						// loop through method parameters
						List<ParameterInfo> parameters = function.GetParameters().ToList();

						for (int i = 0; i < parameters.Count; i++)
						{
							// get param default
							object defaultParameter = parameters[i].DefaultValue;

							// if no default param is specified, set to default of its class
							if (defaultParameter == null)
							{
								defaultParameter = Activator.CreateInstance(parameters[i].ParameterType);
							}

							// add default parameter to array
							parametersProperty.InsertArrayElementAtIndex(i);
							parametersProperty.GetArrayElementAtIndex(i).managedReferenceValue = defaultParameter;
						}

						property.serializedObject.ApplyModifiedProperties();
					});

					// open search window
					SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchWindow);
				}

				// create inputs for each parameter
				if (functionSelected)
				{
					Debug.Log(parametersProperty.arraySize);
					// loop through parameters
					for (int i = 0; i < parametersProperty.arraySize; i++)
					{
						// get parameter property and create a property field for it
						SerializedProperty elementProperty = parametersProperty.GetArrayElementAtIndex(i);
						EditorGUILayout.PropertyField(elementProperty, true);
					}
				}
			}

			// reset indent settings
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}

		private List<MethodInfo> FilterMethodInfosByType(List<MethodInfo> methods, Type returnType)
		{
			return methods.Where(method => method != null && method.ReturnType == returnType).ToList();
		}
	}
}
