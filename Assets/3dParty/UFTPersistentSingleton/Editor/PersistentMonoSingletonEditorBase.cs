using UnityEngine;
using System.Collections;
using UnityEditor;

/*
 * Base class for PersistentMonoSingletonEditor
 * Use this class if you'd like to save|restore properties in Editor mode.
 * This Script will add 3 buttons (save|read|reset) to standard Object Inspector
 * 
 */



public abstract class PersistentMonoSingletonEditorBase<T> : Editor where T:PersistentMonoSingleton<T> {
	T targetObject;
	
	public void OnEnable(){
		targetObject=(T) target;
		targetObject.readPropertiesFromPlayerPrefs();
		EditorUtility.SetDirty(target);
	}
	
	public override void OnInspectorGUI(){
		 
		DrawDefaultInspector ();
		
		
		if(GUILayout.Button ("Save To PlayerPrefs")) 
        	 targetObject.savePropertiesToPlayerPrefs();
		if(GUILayout.Button ("Read From PlayerPrefs")){ 
			
    		targetObject.readPropertiesFromPlayerPrefs();
			EditorUtility.SetDirty(target);
		}
		if(GUILayout.Button ("Reset All Properties")){ 
        	if(EditorUtility.DisplayDialog("!!! ACHTUNG !!!", "It will remove all stored properties in PlayerPrefs, are you sure?", "Yes, I am","No")){
				targetObject.resetAllProperties();
				targetObject.readPropertiesFromPlayerPrefs();
				EditorUtility.SetDirty(target);					
			}
		}
		
		
	}
}
