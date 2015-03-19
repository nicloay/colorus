using System;

/*
 * Custom Atribute, can be used for Class and fields
 * name - will be used to create prefix in PlayerPrefs key value
 * updateOnInstantiation  - used only to mark a class, if you want to read values from PlayerPrefs on singleton instantiation
 * 
 */


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
public class Persistent :Attribute
{
	public string name;
	public bool updateOnInstantiation;
	
	public Persistent(string name="",bool updateOnInstantiation=false)
	{
		this.name=name;
		this.updateOnInstantiation=updateOnInstantiation;
	}
}


