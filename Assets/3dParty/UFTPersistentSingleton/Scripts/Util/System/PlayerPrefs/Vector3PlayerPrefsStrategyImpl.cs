using System;
using UnityEngine;


public class Vector3PlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		Vector3 v3=(Vector3)obj;
		float[] floatArray=new float[]{v3.x,v3.y,v3.z};
		string str=ConverterUtils.convertFloatArrayToString(floatArray);
		PlayerPrefs.SetString(key,str);
	}

	public object readFromPlayerPrefs (string key)
	{
		string str=PlayerPrefs.GetString(key);
		float[] floatArray=ConverterUtils.convertStringToFloatArray(str);
		Vector3 v3=Vector3.zero;
		if (floatArray.Length==3){
			v3=new Vector3(floatArray[0],floatArray[1],floatArray[2]);			
		}
		return (object)v3;
		
	}
	#endregion
	
}


