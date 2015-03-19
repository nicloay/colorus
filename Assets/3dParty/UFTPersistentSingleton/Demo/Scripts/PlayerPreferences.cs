using UnityEngine;
using System.Collections;

/*
 * Persistent("playerSetup",true)  
 *   1st parameter, prefix for stored values, 
 *   2d parameter do you want automaticaly read values from PlayerPrefs on singleton initializaiton or not 
 * Default values for class
 * Persistent("",false) which means, no prefix in PlayerPrefs, don't update automaticaly.
 * 
 * 
 * In current example All properties will be automaticaly populated from PlayerPrefs on singleton initialisation
 * properties will be stored with following keys
 * 
 * playerSetup.myIntVar
 * playerSetup.myFloatVar
 * playerSetup.privateField
 * playerSetup.myString         //because we didn't provide the name
 * 
 */ 

[Persistent("playerSetup",true)]
public class PlayerPreferences 
	: PersistentMonoSingleton<PlayerPreferences> {
	
	[Persistent("myIntVar")]
	public int myIntVar;
	
	
	[Persistent("myFloatVar")]
	public float myFloatVar;
	
	
	[Persistent("privateField")]
	private int myPrivateInt;
	
	[Persistent]
	public string myString;
	
	[Persistent]
	public bool myBool;
		
	[Persistent]		
	public Vector2 myVector2;
	
	[Persistent]
	public Vector3 myVector3;
	
	
	[Persistent]
	public Quaternion myQuaternion;
	
	[Persistent]
	public Color myColor;
		
}
