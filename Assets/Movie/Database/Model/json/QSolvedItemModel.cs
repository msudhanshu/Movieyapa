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
public class QSolvedItemModel : IComparable<QSolvedItemModel> {

	public int id { get; set; }
	// if requried in future public int state {get; set;}
	public int level {get; set;}

	public QSolvedItemModel() {

	}

	public QSolvedItemModel(int id, int level) {
			this.id = id;
			this.level = level;
	}

//	public QSolvedItemModel(string id, string level) {
//			this.id = Convert.ToInt32(id);
//			this.level = Convert.ToInt16(level);
//		}

	public int CompareTo(QSolvedItemModel comparePart)
	{
		// A null value means that this object is greater.
		if (comparePart == null)
			return 1;
		else
			return this.level.CompareTo(comparePart.level);
	}
}




