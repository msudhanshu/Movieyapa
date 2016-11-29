//
//  IActivityTarget.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//

using System;
using UnityEngine;


public interface IActivityTarget
{
	ActivityController activityController{get;}
	Vector3 targetWorldPosition {get;}
	GridPosition targetGridPosition{get;}
	bool IsHelperRequired();
	void StartActivity();
}


