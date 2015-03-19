using UnityEngine;
using System.Collections;

public class LayerUtil {
	
	
	
	public static GameObject createLayer (string shaderName, string name, Vector3 position, Transform parentT)
	{
		GameObject go = MeshUtil.createPlane(PropertiesSingleton.instance.width,PropertiesSingleton.instance.height,name);
		go.name=name;
#if UNITY_EDITOR
		go.transform.renderer.sharedMaterial=MaterialUtil.getMaterial(go.name,shaderName);				
#endif
		go.transform.parent=parentT;
		go.transform.position=position;		
		return go;
	}
	
	public static GameObject createBufferLayer (string shaderName, string name, Vector3 position, Transform parentT){		
		GameObject go=createLayer(shaderName,name,position,parentT);
		
		Texture2D texture= TextureUtil.createAndSaveAlpha8Texture(PropertiesSingleton.instance.width,PropertiesSingleton.instance.height,Color.clear,name);
		go.transform.renderer.sharedMaterial.SetTexture("_MainTex",texture);	
		return go;		
	}
	
	public static GameObject createBackLayer (string shaderName, string name, Vector3 position, Transform parentT){		
		GameObject go=createLayer(shaderName,name,position,parentT);
		Color color=Color.white;
		Texture2D texture= TextureUtil.createAndSaveAlphaRGBA32Texture(PropertiesSingleton.instance.width,PropertiesSingleton.instance.height,color,name);
		go.transform.renderer.sharedMaterial.SetTexture("_MainTex",texture);	
		return go;		
	}
	
	
}
