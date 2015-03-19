using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MaterialUtil {
	public static string ASSET_MATERIAL_HOME = "Assets/Resources/Materials/";
	
	
#if UNITY_EDITOR
	public static Material getMaterial(string name,string shaderPath){		
		Material mat=(Material)AssetDatabase.LoadAssetAtPath(ASSET_MATERIAL_HOME+name+".mat",typeof(Material));
		if (mat ==null){
			Shader shdr=Shader.Find(shaderPath);					
			mat = new Material(shdr);
		}
		return mat;
	}
#endif
	
	
}
