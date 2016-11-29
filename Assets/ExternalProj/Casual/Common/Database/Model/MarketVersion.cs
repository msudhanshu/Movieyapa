using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class MarketVersion : BaseDbModel{
	[PrimaryKey]
	public int id {get; set;}
	public int version {get; set;}
}
