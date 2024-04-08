using UnityEngine;
using UnityEditor;
using System.Reflection.Emit;
using Codice.CM.Client.Gui;
using Mono.Cecil.Cil;

namespace ReturnableUnityEvents
{
	[CustomPropertyDrawer(typeof(ReturnableUnityEvent<>))]
	public class ReturnableUnityEventDrawer : PropertyDrawer
	{
		Object targetObject;

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
			EditorGUI.ObjectField(pos1, targetObject, typeof(Object), true);

			// function selection (disabled by default)
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.Popup(pos2, 0, new GUIContent[] {
				new GUIContent("No Function")
			});
			EditorGUI.EndDisabledGroup();

			// reset indent settings
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
