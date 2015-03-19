using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class GUIStamp{
	Texture2D _iconTexture;
	Texture2D _stampTexture;
	
	public string iconPath;
	public string stampPath;

	public AudioClip sound;

	public Texture2D iconTexture{
		get{
			if (_iconTexture == null)
				_iconTexture = (Texture2D)Resources.Load(iconPath);
			return _iconTexture;
		}
	}
	
	public Texture2D stampTexture{
		get{
			if(_stampTexture == null)
				_stampTexture = (Texture2D)Resources.Load(stampPath);
			return _stampTexture;
		}
	}

	public void releaseTextures(){
		if (_iconTexture!=null)
			Resources.UnloadAsset(_iconTexture);
		if (_stampTexture!=null)
			Resources.UnloadAsset(_stampTexture);
		
	}




}

[Serializable]
public class GUIStampList : ScriptableObject {
	public List<GUIStamp> stampList;

}
