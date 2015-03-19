using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;


public class BulkPicsParseWizard : ScriptableWizard  {
	public string texturePath = "Assets/Assets/Textures/animals";
	public string objDestination = "Assets/Assets/ScriptableObjects/Animals";
	public string listDestination = "Assets/Assets/ScriptableObjects";
	public List<string> names;
	
	[MenuItem ("Razukrashka/PicsBulkCreate", false, 3)]
	static void CreateWizard () {
		BulkPicsParseWizard wiz= ScriptableWizard.DisplayWizard<BulkPicsParseWizard>("Razukrashka/PicsBulkCreate", "Create");        
		wiz.initialize();
	}
	
	
	public void initialize(){		
		names = new List<string>();
		foreach (string name in Directory.GetDirectories(texturePath)){
			if (name.EndsWith(".psd") && !name.Contains("icon")){
				string filename=Path.GetFileNameWithoutExtension(name);
				names.Add(   filename.Replace("stamp",""));	
			}			
		}					
	}
	
	void OnWizardCreate () {

		
		string[] dirs = Directory.GetDirectories(texturePath);
		
		SheetList sl =ScriptableObjectUtility.CreateAssetAtPath<SheetList>("Songs",listDestination);
		SheetObject[] sheetArray = new SheetObject[dirs.Length];
		
		for (int i = 0; i < dirs.Length; i++) {
			string name = new DirectoryInfo(dirs[i]).Name;
			Debug.Log("name = "+name);
			SheetObject so = ScriptableObjectUtility.CreateAssetAtPath<SheetObject> (name,objDestination);			
			so.persistentBorderLayerPath = name+".border";
			so.persistentFrontOutlinePath = name+".outline";
			sheetArray[i]=so;
			EditorUtility.SetDirty(so);
		}
		sl.sheetList = sheetArray;
		EditorUtility.SetDirty(sl);
		AssetDatabase.SaveAssets();
	} 
	
}
