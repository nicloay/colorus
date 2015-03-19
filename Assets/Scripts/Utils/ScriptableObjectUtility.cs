#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using UnityEditor;


/*
 * source code by Brandon Edmark from 
 * http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
 * 
 */ 
 
public static class ScriptableObjectUtility
{
	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
 
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
 
		AssetDatabase.CreateAsset (asset, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
	
	
	public static T CreateAsset<T> (string name) where T : ScriptableObject
	{
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}		
		return CreateAssetAtPath<T>(name,path);
	}
	
	
	public static T CreateAssetAtPath<T> (string name, string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + name + ".asset");
 
		AssetDatabase.CreateAsset (asset, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		return asset;
	}
}

#endif