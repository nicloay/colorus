using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
 
public class AssetCreateMenuItem
{
	[MenuItem("Assets/Create/SheetObject")]
	public static void CreateSheetAsset ()
	{
		ScriptableObjectUtil.CreateAsset<SheetObject> ();
	}
	
	
	[MenuItem("Razukrashka/Sheet/Create SheetListObject")]
	public static void CreateSheetListAsset ()
	{
		ScriptableObjectUtil.CreateAsset<SheetList> ();
	}

	[MenuItem("Razukrashka/Stamp/Create GUIStampListObject")]
	public static void CreateGUIStampListAsset(){
		GUIStampList list =  ScriptableObjectUtil.CreateAsset<GUIStampList>();
		list.stampList = new List<GUIStamp>();
		string stampFolder = "Assets/Assets/Textures/stamps/Resources";
		
		string[] filePaths = Directory.GetFiles(stampFolder);
		for (int i = 0; i < filePaths.Length; i++) {
			string filePath = filePaths[i];
			if (!filePath.EndsWith(".meta")){				
				string fileName = Path.GetFileName(filePath);
				if (!fileName.StartsWith("stamp"))
					continue;
				GUIStamp stampObj = new GUIStamp();
				stampObj.stampPath = fileName.Replace(".psd","");
				stampObj.iconPath = stampObj.stampPath+".icon";
				list.stampList.Add(stampObj);
			}
		}
		EditorUtility.SetDirty(list);
		
	}
	
	
	[MenuItem("Razukrashka/Albums")]
	public static void CreateAlbumsAsset ()
	{
		ScriptableObjectUtil.CreateAsset<Albums> ();
	}
	
}
