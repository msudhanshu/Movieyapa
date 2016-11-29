using System;
using System.Reflection;
using System.Collections.Generic;
using Expansion;
	
public class IoCContainer
{
	Dictionary<Type,Type> modelMap;
	private IoCContainer ()
	{
		modelMap = new Dictionary<Type, Type>();
	}
	private static IoCContainer _instance;
	public static IoCContainer GetInstance(){
		if (_instance == null) {
			_instance = new IoCContainer();
		}
		return _instance;
	}

	public void RegisterType(Type iType, Type cType){
		MethodInfo mInfo = cType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		if (mInfo == null) {
			throw new Exception ("Can't find static GetInstance method for type: " + cType);
			return;
		}
		modelMap.Add (iType, cType);
	}

	public T GetImpl<T>(){
		if (modelMap != null && modelMap.ContainsKey(typeof(T))) {
			Type cType = modelMap [typeof(T)];
			if (cType != null) {
				MethodInfo mInfo = cType.GetMethod ("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (mInfo != null) {
					Object impl = mInfo.Invoke (null, null);
					if (impl != null)
						return ((T)(impl));
				}
			}
		}
		return default(T);
	}

}


