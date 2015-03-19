using System;
using UnityEngine;


public class Vector2PlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		Vector2 v2=(Vector2)obj;
		float[] floatArray=new float[]{v2.x,v2.y};
		string str=ConverterUtils.convertFloatArrayToString(floatArray);
		PlayerPrefs.SetString(key,str);
	}

	public object readFromPlayerPrefs (string key)
	{
		string str=PlayerPrefs.GetString(key);
		float[] floatArray=ConverterUtils.convertStringToFloatArray(str);
		Vector2 v2=Vector2.zero;
		if (floatArray.Length==2){
			v2=new Vector2(floatArray[0],floatArray[1]);
			
		}
		return (object)v2;
		
	}
	#endregion
	
}


