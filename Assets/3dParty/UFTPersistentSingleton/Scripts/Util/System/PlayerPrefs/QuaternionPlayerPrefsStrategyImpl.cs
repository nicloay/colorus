using System;
using UnityEngine;


public class QuaternionPlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		Quaternion q=(Quaternion)obj;
		float[] floatArray=new float[]{q.x,q.y,q.z,q.w};
		string str=ConverterUtils.convertFloatArrayToString(floatArray);
		PlayerPrefs.SetString(key,str);
	}

	public object readFromPlayerPrefs (string key)
	{
		string str=PlayerPrefs.GetString(key);
		float[] floatArray=ConverterUtils.convertStringToFloatArray(str);
		Quaternion q=Quaternion.identity;
		if (floatArray.Length==4){
			q=new Quaternion(floatArray[0],floatArray[1],floatArray[2],floatArray[3]);			
		}
		return (object)q;
		
	}
	#endregion
	
}


