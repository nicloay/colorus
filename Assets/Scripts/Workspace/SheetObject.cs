using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class SheetObject : ScriptableObject {
	public string           nameKey;
	public Texture2D             persistentBorderLayer{
		get {
			if (_persistentBorderLayerCache == null)
				_persistentBorderLayerCache = (Texture2D) Resources.Load (persistentBorderLayerPath);
			return _persistentBorderLayerCache;
		}
	}
	public Texture2D             persistentFrontOutline{
		get {
			if (_persistentFrontOutlineCache == null)
				_persistentFrontOutlineCache = (Texture2D) Resources.Load(persistentFrontOutlinePath);
			return _persistentFrontOutlineCache;
		}
	}

	public string                persistentBorderLayerPath;
	public string                persistentFrontOutlinePath;
	
	private Texture2D             _persistentBorderLayerCache  ;
	private Texture2D             _persistentFrontOutlineCache ;

	public void releaseTextures(){
		if (_persistentBorderLayerCache!=null)
			Resources.UnloadAsset(_persistentBorderLayerCache);
		if (_persistentFrontOutlineCache!=null)
			Resources.UnloadAsset(_persistentFrontOutlineCache);
		
	}
	
#if UNITY_EDITOR
	[MenuItem("Razukrashka/Sheet/Create SheetObject")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<SheetObject> ();
	}
#endif
}
