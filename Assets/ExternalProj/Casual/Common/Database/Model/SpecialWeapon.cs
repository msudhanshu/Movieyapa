using UnityEngine;
using System.Collections;
using SimpleSQL;
using KiwiCommonDatabase;

public class SpecialWeapon : BaseDbModel
{
	// The WeaponID field is set as the primary key in the SQLite database,
	// so we reflect that here with the PrimaryKey attribute
	[PrimaryKey]
	public int id { get; set; }
	public string WeaponName { get; set; }
//	public float Damage { get; set; }
//	public float Cost { get; set; }
//	public float Weight { get; set; }
//	public int WeaponTypeID { get; set; }
	public string WeaponTypeDescription { get; set; }

	public override string ToString(){
		return "{SpecialWeapon: id=" + id + ", weaponName=" + WeaponName + ", weaponTypeDescription=" + WeaponTypeDescription + "}";
	}
}

