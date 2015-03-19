using System;
using UnityEngine;


public class StringPlayerPrefsStrategyImpl:PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		PlayerPrefs.SetString(key,(string)obj);
	}

	public object readFromPlayerPrefs (string key)
	{
		return PlayerPrefs.GetString(key);
	}
	#endregion
	
}

