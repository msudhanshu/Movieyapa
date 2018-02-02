using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expansion;

//firebase
//public class UserQSolvedModel {
//	public List<QSolvedItem> qsolved {get; set;}
//
public class QLevelItemModel : /*SgFirebaseDataInitialiser.IFireBaseModel<int>,*/ IEquatable<QLevelItemModel> , IComparable<QLevelItemModel> {
		
//	public int itemId {
//		get {
//			return id;
//		}
//		set {
//			id = value;
//		}
//	}

	public int id { get; set; }
	public int level {get; set;}

	public QLevelItemModel() {

	}

	public QLevelItemModel(int id, int level) {
			this.id = id;
			this.level = level;
	}

//	public QLevelItemModel(string id, string level) {
//			this.id = Convert.ToInt32(id);
//			this.level = Convert.ToInt16(level);
//	}


	// Default comparer for Part type.
	public int CompareTo(QLevelItemModel comparePart)
	{
		// A null value means that this object is greater.
		if (comparePart == null)
			return 1;
		else
			return this.level.CompareTo(comparePart.level);
	}

	public override string ToString()
	{
		return "ID: " + id + "   Level: " + level;
	}

	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		QLevelItemModel objAsPart = obj as QLevelItemModel;
		if (objAsPart == null) return false;
		else return Equals(objAsPart);
	}

	public int SortByNameAscending(string name1, string name2)
	{
		return name1.CompareTo(name2);
	}

	public override int GetHashCode()
	{
		return id;
	}
	public bool Equals(QLevelItemModel other)
	{
		if (other == null) return false;
		return (this.id.Equals(other.id));
	}

}



