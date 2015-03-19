using UnityEngine;
using UnityEditor;
using System.Collections;

public class TexturePreprocessor : AssetPostprocessor {
	public static string picturePath="Assets/Assets/Textures";
	public static string picsFolder = "Assets/Assets/Textures/Pictures";
	public static string BORDER_POSTFIX = ".border";
	public static string stampPath = "Assets/Assets/Textures/stamps";


	void OnPostprocessTexture(Texture2D texture){
		if (assetPath.Contains(picsFolder)){
			TextureImporter importer = assetImporter as TextureImporter;
			importer.isReadable = true;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.anisoLevel = 0;
			importer.mipmapEnabled = false;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.filterMode = FilterMode.Point;
			if (assetPath.Contains (stampPath)) {
				importer.maxTextureSize = 256;
			} else {
				importer.maxTextureSize = 1024;
			}


			if (assetPath.Contains (picsFolder) && assetPath.Contains (BORDER_POSTFIX)) {
				importer.textureFormat = TextureImporterFormat.Alpha8;
			} else {
				importer.textureFormat = TextureImporterFormat.RGBA32;
			}
		}


	}
}
