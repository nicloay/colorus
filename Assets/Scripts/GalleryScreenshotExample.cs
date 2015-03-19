using UnityEngine;
using System.Collections;
using System.IO;

public class GalleryScreenshotExample : MonoBehaviour {
	
	public Texture2D texture;
	bool saved = false;
	bool saved2 = false;
	
	void Start ()
	{
		ScreenshotManager.ScreenshotFinishedSaving += ScreenshotSaved;	
		ScreenshotManager.ImageFinishedSaving += ImageSaved;
	}
	
	void OnGUI ()
	{
		GUILayout.Label("Example scene showing: \n1. how to save a screenshot\n" +
						"2. how to save an image from your assets");
		
		if(GUILayout.Button ("Take Screenshot", GUILayout.Width (200), GUILayout.Height(80)))
		{	
			StartCoroutine(ScreenshotManager.Save("MyScreenshot", "MyApp", true));
		}
		
		if(saved) GUILayout.Label ("Screenshot was successfully saved");
		
		GUILayout.Space(40);
		
		GUILayout.Label(texture);
		
		if(GUILayout.Button ("Save " + texture.name, GUILayout.Width (200), GUILayout.Height(80)))
		{	
			StartCoroutine("SaveAssetImage");
		}
		
		if(saved2) GUILayout.Label(texture.name + " was successfully saved");
	}
	
	IEnumerator SaveAssetImage ()
	{
		byte[] bytes = texture.EncodeToPNG();
		string path = Application.persistentDataPath + "/MyImage.png";
		File.WriteAllBytes(path, bytes);
		
		yield return new WaitForEndOfFrame();
		
		StartCoroutine(ScreenshotManager.SaveExisting(path, true));
	}
	
	void ScreenshotSaved()
	{
		Debug.Log ("screenshot finished saving");
		saved = true;
	}
	
	void ImageSaved()
	{
		Debug.Log (texture.name + " finished saving");
		saved2 = true;
	}
}