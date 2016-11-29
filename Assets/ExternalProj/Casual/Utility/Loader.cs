using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
#if ENABLE_JSON
using Newtonsoft.Json;
#endif

/**
 * The loader is a generic class responsible for serializing and deserializing data
 * to XML files.
 */
public class Loader <T> {
	
	/**
	 * Load data from an XML resource.
	 * 
	 * @param	resourceName	The name of the resource file.
	 * 
	 * @return 	A List containing the loaded T.
	 */

	public List<T> Load(string resourceName) {
		#if ENABLE_JSON
		return LoadJSON(resourceName);
		#else
		return LoadXML(resourceName);
		#endif
	}

	public List<T> LoadXML(string resourceName) {
		TextAsset asset = Resources.Load(resourceName) as TextAsset;
		using (Stream stream = new MemoryStream(asset.bytes)){
			Type[] types = typeof(T).Assembly.GetTypes().Where(t => t != typeof(T) && typeof(T).IsAssignableFrom(t)).ToArray();
			XmlSerializer serializer = new XmlSerializer(typeof(List<T>), types);
			List<T>  itemData = (List<T> ) serializer.Deserialize(stream);
			return itemData;
		}
	}

	public List<T> LoadJSON(string resourceName) {
#if ENABLE_JSON
		TextAsset asset = Resources.Load(resourceName) as TextAsset;
		if(asset==null) return null;
		return JsonConvert.DeserializeObject<List<T>>(asset.text);
#endif
		return null;
	}
	
	/**
	 * Save a list of data to a file. Note that this does not save to a resource
	 * although you could specify a resource folder in the fileName.
	 * 
	 * @param 	data		The data to save.
	 * @param	fileName	Name and path of the file to save as.
	 */ 
	//TODO : can add file extension here
	public void Save(List<T> data, string fileName){
		#if ENABLE_JSON
		SaveJSON(data,fileName);
		#else
		SaveXML(data,fileName);
		#endif
	}

	public void SaveXML(List<T> data, string fileName){
		Type[] types = typeof(T).Assembly.GetTypes().Where(t => t != typeof(T) && typeof(T).IsAssignableFrom(t)).ToArray();
		XmlSerializer serializer = new XmlSerializer(typeof(List<T> ), types);
    	using (Stream stream = new FileStream(fileName, FileMode.CreateNew)){
    		serializer.Serialize(stream, data);
		}
	}

	public void SaveJSON(List<T> data, string fileName){
#if ENABLE_JSON
		string jsonStr = JsonConvert.SerializeObject(data);
		Debug.Log("jsonStr:" + jsonStr);
		FileStream stream = new FileStream(fileName, FileMode.Create);
		System.IO.StreamWriter file = new System.IO.StreamWriter(stream);
		file.WriteLine(jsonStr);
		file.Close();
#endif
	}

}
