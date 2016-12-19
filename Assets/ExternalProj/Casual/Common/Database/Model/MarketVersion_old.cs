using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class MarketVersion_old : BaseDbModel{
	[PrimaryKey]
	public int id {get; set;}
	public int version {get; set;}
}
