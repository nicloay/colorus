using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(MainOnGUI))]
public class MainOnGUIEditor : Editor {
	bool debugMode;
	
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		debugMode = EditorGUILayout.Foldout(debugMode,"debug");
		if (debugMode){
			EditorGUI.indentLevel++;
			if (GUILayout.Button("simulate newColor press"))
				simulateNewColorPress();
			
			EditorGUI.indentLevel--;			
		}
	}
	
	void simulateNewColorPress(){
		
	}
}
