using UnityEngine;
using UnityEditor;
using System.Collections;
using localisation;
using System.Collections.Generic;

public class LanguageMenuItem {

	[MenuItem("Assets/Create/Language File")]
	public static void newLocalisationFile(){
		ScriptableObjectUtility.CreateAsset<LanguageFile>();
	}
}
