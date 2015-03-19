#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class ScreenshotManager : MonoBehaviour {
	
	public static event Action ScreenshotFinishedSaving;
	public static event Action ImageFinishedSaving;
	
	#if UNITY_IPHONE
	
	[DllImport("__Internal")]
    private static extern bool saveToGallery( string path );
	
	#endif

	public static string getName(string name){
		string date = System.DateTime.Now.ToString("dd-MM-yy");
		
		ScreenshotManager.ScreenShotNumber++;
		return  name + " " + ScreenshotManager.ScreenShotNumber + "_" + date + ".png";
	}


	public static IEnumerator Save(string fileName, string albumName = "MyScreenshots", bool callback = false)
	{
		bool photoSaved = false;
		

		
		string screenshotFilename = getName(fileName);
		
		Debug.Log("Save screenshot " + screenshotFilename); 
		
		#if UNITY_IPHONE
		
			if(Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				Debug.Log("iOS platform detected");
				
				string iosPath = Application.persistentDataPath + "/" + screenshotFilename;
		
				Application.CaptureScreenshot(screenshotFilename);
				
				while(!photoSaved) 
				{
					photoSaved = saveToGallery( iosPath );
					
					yield return null;
				}
			
				iPhone.SetNoBackupFlag( iosPath );
			
			} else {
			
				Application.CaptureScreenshot(screenshotFilename);
			
			}
			
		#elif UNITY_ANDROID	
				
			if(Application.platform == RuntimePlatform.Android) 
			{
				Debug.Log("Android platform detected");
				
				string androidPath = "/../../../../DCIM/" + albumName + "/" + screenshotFilename;
				string path = Application.persistentDataPath + androidPath;
				string pathonly = Path.GetDirectoryName(path);
				Directory.CreateDirectory(pathonly);
				Application.CaptureScreenshot(androidPath);
				
				AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
				
				while(!photoSaved) 
				{
					photoSaved = obj.CallStatic<bool>("scanMedia", path);
				
					yield return null;
				}
		
			} else {
		
				Application.CaptureScreenshot(screenshotFilename);
		
			}
		#else
			
			while(!photoSaved) 
			{
				yield return null;
		
				Debug.Log("Screenshots only available in iOS/Android mode!");
			
				photoSaved = true;
			}
		
		#endif
		
		if(callback)
			ScreenshotFinishedSaving();
	}
	
	
	public static IEnumerator SaveExisting(string filePath, bool callback = false)
	{
		bool photoSaved = false;
		
		//Debug.Log("Save existing file to gallery " + filePath);

		#if UNITY_IPHONE
		
			if(Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				Debug.Log("iOS platform detected");
				
				while(!photoSaved) 
				{
					photoSaved = saveToGallery( filePath );
					
					yield return new WaitForSeconds(.1f);
				}
			
				iPhone.SetNoBackupFlag( filePath );
			}
			
		#elif UNITY_ANDROID	
				
			if(Application.platform == RuntimePlatform.Android) 
			{
				Debug.Log("Android platform detected");

				AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
					
				while(!photoSaved) 
				{
					photoSaved = obj.CallStatic<bool>("scanMedia", filePath);
							
					yield return new WaitForSeconds(0.1f);
				}
			
			}
		
		#else
			
			while(!photoSaved) 
			{
				yield return null;
		
				Debug.Log("Save existing file only available in iOS/Android mode!");

				photoSaved = true;
			}
		
		#endif
		
		if(callback)
			ImageFinishedSaving();
	}
	
	
	public static int ScreenShotNumber 
	{
		set { PlayerPrefs.SetInt("screenShotNumber", value); }
	
		get { return PlayerPrefs.GetInt("screenShotNumber"); }
	}
}
