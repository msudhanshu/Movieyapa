using UnityEngine;
using System.Collections;
using SimpleSQL;

[System.Serializable]
public class UserCollectable {
	public int id {get; set;}
	public string collectableId {get; set;}

	private Collectable _collectable;
	public Collectable collectable {
		get {
			if (_collectable == null)
				_collectable = DatabaseManager.GetCollectable(collectableId);
			return _collectable;
		}
		set {
			collectableId = value.id;
		}
	}

	public int count {get; set;}

}