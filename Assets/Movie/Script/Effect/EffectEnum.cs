using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public enum EffectEnum  {
    [StringValue("NONE")]
	NONE,
    [StringValue("DEFAULT")]
	DEFAULT,
    [StringValue("GRAY")]
	GRAY,
    [StringValue("EDGE_DETECT")] //Shader("EffectEdgeDetect"), 
	EDGE_DETECT,
    [StringValue("DOUBLE_VISION")]
	DOUBLE_VISION
}

/*
public static class EffectEnumExtensions
{

	public static string GetShader(this EffectEnum value)
	{
	
		Type type = value.GetType();
		FieldInfo fieldInfo = type.GetField(value.ToString());
		// Get the stringvalue attributes
		ShaderAttribute[] attribs = fieldInfo.GetCustomAttributes(
			typeof(ShaderAttribute), false) as ShaderAttribute[];
		// Return the first if there was a match.
		return attribs.Length > 0 ? attribs[0].Value : null;

		//string attr = p.GetAttribute<ShaderAttribute>();
		//return attr;
	}
}


public class ShaderAttribute : System.Attribute
{
	
	private string _value;
	
	public ShaderAttribute(string value)
	{
		_value = value;
	}
	
	public string Value
	{
		get { return _value; }
	}
	
}
*/