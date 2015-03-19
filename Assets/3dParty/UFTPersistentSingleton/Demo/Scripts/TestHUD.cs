using UnityEngine;
using System.Collections;
using System;

public class TestHUD : MonoBehaviour {

	// Update is called once per frame
	void OnGUI () {
		
		int startY=10;
		int offset=25;
		
		//int var
		try{
			GUI.Label(new Rect(10, startY, 100, 20),"myIntVar");
			int myNewIntVar = int.Parse( GUI.TextField(new Rect(110, startY, 200, 20), ""+PlayerPreferences.instance.myIntVar , 25));
			PlayerPreferences.instance.myIntVar=myNewIntVar;
		} catch {}
		
		
		//float var
		startY+=offset;		
		try{
			GUI.Label(new Rect(10, startY, 100, 20),"myFloatVar");
			float myNewFloatVar = float.Parse( GUI.TextField(new Rect(110, startY, 200, 20), ""+PlayerPreferences.instance.myFloatVar , 25));
			PlayerPreferences.instance.myFloatVar=myNewFloatVar;
		} catch {}
		
		
		//string var
		startY+=offset;		
		GUI.Label(new Rect(10, startY, 100, 20),"myString");
		string myNewString = GUI.TextField(new Rect(110, startY, 200, 20), PlayerPreferences.instance.myString , 25);
		PlayerPreferences.instance.myString = myNewString;
		
		
		//bool var
		startY+=offset;		
		GUI.Label(new Rect(10, startY, 100, 20),"myBool");
		bool myBool=GUI.Toggle(new Rect(110, startY, 200, 20), PlayerPreferences.instance.myBool, "");						
		PlayerPreferences.instance.myBool = myBool;
		
		
		//Vector2 var
		startY+=offset;		
		try{
			GUI.Label(new Rect(10, startY, 100, 20),"myVector2");
			Vector2 v2=PlayerPreferences.instance.myVector2;
			GUI.Label(new Rect(110, startY, 10, 20),"x");
			float x = float.Parse( GUI.TextField(new Rect(120, startY, 80, 20), ""+v2.x , 25));
			GUI.Label(new Rect(210, startY, 10, 20),"y");
			float y = float.Parse( GUI.TextField(new Rect(220, startY, 90, 20), ""+v2.y , 25));			
			PlayerPreferences.instance.myVector2=new Vector2(x,y);
		} catch {}
		
		
		//Vector3 var
		startY+=offset;		
		try{
			GUI.Label(new Rect(10, startY, 100, 20),"myVector3");
			Vector3 v3=PlayerPreferences.instance.myVector3;
			GUI.Label(new Rect(110, startY, 10, 20),"x");
			float x = float.Parse( GUI.TextField(new Rect(120, startY, 80, 20), ""+v3.x , 25));
			GUI.Label(new Rect(210, startY, 10, 20),"y");
			float y = float.Parse( GUI.TextField(new Rect(220, startY, 90, 20), ""+v3.y , 25));			
			GUI.Label(new Rect(310, startY, 10, 20),"y");
			float z = float.Parse( GUI.TextField(new Rect(320, startY, 90, 20), ""+v3.z , 25));			
			
			
			
			PlayerPreferences.instance.myVector3=new Vector3(x,y,z);
		} catch {}
		
		
		
		
		
		startY+=offset;
		if (GUI.Button(new Rect(10, startY, 90, 20),"Save")){
			PlayerPreferences.instance.savePropertiesToPlayerPrefs();	
		}
		if (GUI.Button(new Rect(110, startY, 90, 20),"Read")){
			PlayerPreferences.instance.readPropertiesFromPlayerPrefs();
		}
		if (GUI.Button(new Rect(210, startY, 90, 20),"Reset")){
			PlayerPreferences.instance.resetAllProperties();
		}
	}
	

	
}
