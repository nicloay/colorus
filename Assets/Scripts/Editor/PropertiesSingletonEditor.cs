using UnityEngine;
using System.Collections;
using UnityEditor;


/*
 * Demo Class, just show editor, 
 * how to create editor for your singleton, after that it will support (save/read/reset) operation
 * 
 */

[CustomEditor(typeof(PlayerPreferences))]
public class PlayerPreferencesEditor 
	: PersistentMonoSingletonEditorBase<PlayerPreferences>{
	
}
