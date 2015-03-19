using System;
using UnityEngine;


public class IntPlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		PlayerPrefs.SetInt(key, (int)obj);
	}

	public object readFromPlayerPrefs (string key)
	{
		return PlayerPrefs.GetInt(key);
	}
	#endregion
	
	
}


