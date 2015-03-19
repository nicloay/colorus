using System;
using UnityEngine;


public class BoolPlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		bool boolObj=(bool)obj;
		PlayerPrefs.SetInt(key,boolObj?1:0);
		
	}

	public object readFromPlayerPrefs (string key)
	{
		int result=PlayerPrefs.GetInt(key);
		return (object)(result==1);		
	}
	#endregion
	
}


