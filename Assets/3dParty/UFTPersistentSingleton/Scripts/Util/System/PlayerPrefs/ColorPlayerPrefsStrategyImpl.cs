using System;
using UnityEngine;


public class ColorPlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		Color color=(Color)obj;
		float[] floatArray=new float[]{color.r,color.g,color.b,color.a};
		string str=ConverterUtils.convertFloatArrayToString(floatArray);
		PlayerPrefs.SetString(key,str);
	}

	public object readFromPlayerPrefs (string key)
	{
		string str=PlayerPrefs.GetString(key);
		float[] floatArray=ConverterUtils.convertStringToFloatArray(str);
		Color color=Color.white;
		if (floatArray.Length==4){
			color=new Color(floatArray[0],floatArray[1],floatArray[2],floatArray[3]);			
		}
		return (object)color;
		
	}
	#endregion
	
}


