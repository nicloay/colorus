using UnityEngine;
using System.Collections;
using UnityEditor;

namespace localisation{
	
	[CustomPropertyDrawer(typeof(string))]
	public class StringEditor : PropertyDrawer{
		InspectorTooltipWindow window;
		public bool closeToolTip=false;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
			position.width-=position.height;
			EditorGUI.PropertyField(position,property,label);
			Rect buttonRect = new Rect(position.width+position.x,position.y,position.height,position.height);

			if (GUI.Button(buttonRect,"L"))
				showToolTip(property.stringValue, position);
			if (closeToolTip){
				closeToolTip = false;
				doCloseToolTip();
			}
		}
		
		void showToolTip(string key, Rect buttonRect){
			Rect parentRect = EditorWindow.focusedWindow.position;
			buttonRect.x+=parentRect.x;
			window = EditorWindow.GetWindow<InspectorTooltipWindow>();
			window.setParameters(key,Locale.instance);
			window.pd = this;
			window.Show();
			//window.ShowAsDropDown(buttonRect,size);
		}

		public void doCloseToolTip(){
			if (window!=null)
				window.Close();
			window=null;
		}
	}
}