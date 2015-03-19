using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class Album{
	public Texture2D icon;
	public SheetObject sheetObject;
	public SheetList sheetList;
}


[Serializable]
public class Albums : ScriptableObject {
	public List<Album> album;	
}
