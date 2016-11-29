using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MazeBlock : BaseBlock, IPointerClickHandler
{
	public void Init(MazeBlockType blockType) {
		this.mazeBlockType = blockType;
		if(MazeManager.GetInstance().mazeMap.allSinkTarget.Contains(new MazePoint(x,y)))
			IsSinkTarget = true;
		if(MazeBlock.IsSinkBlock(blockType)) IsSinkTarget = true;
	}
	
	public bool _IsSinkTarget = false;
	public bool IsSinkTarget{
		get{
			return _IsSinkTarget;
		} 
		set{
			_IsSinkTarget = value;
		}
	}

	public MazeBlockType mazeBlockType = MazeBlockType.FREE;
	public MazePathType mazeEndType = MazePathType.NULL;


	virtual public IEnumerator PathTraceEffect(MazeBlockType blockType){
		SetBlockEffect(blockType);
		yield break;
	}

	virtual public void SetBlockEffect(){
		SetBlockEffect(mazeBlockType);
	}

	//TODO : effect manager
	virtual protected void SetBlockEffect(MazeBlockType blockType){
		renderer.material.color = MazeBlockEffect.GetPathEffect(blockType).color;
	}

	virtual public void OnPointerClick (PointerEventData eventData){
//		Debug.Log("BLock clicked::::"+name);
		//if(IsSinkTarget) {
		if(!IsSourceBlock(mazeBlockType)) {
			MazeManager.GetInstance().gamePlayer.OnSinkBlockSelected(this, mazeBlockType);
//			Debug.Log("BLock  isSinked clicked::::"+name);
		}
	}


	#region "Static_Util_Function"
	public static bool IsBlock(MazeBlockType blockType) {
		return MazeBlockType.BLOCK1==blockType;
	}
	
	public static bool IsAnyBlock(MazeBlockType blockType) {
		return MazeBlockType.FREE!=blockType;
	}
	
	public static bool IsFree(MazeBlockType blockType) {
		return false;
	}
	
	public static bool IsSourceBlock(MazeBlockType blockType) {
		if(blockType == MazeBlockType.SOURCE1 || blockType == MazeBlockType.SOURCE2) return true;
		return false;
	}
	
	public static bool IsSinkBlock(MazeBlockType blockType) {
		if(blockType == MazeBlockType.SINK1 || blockType == MazeBlockType.SINK2) return true;
		return false;
	}
	
	public static MazeBlockType GetSinkForSourceBlock(MazeBlockType blockType) {
		if(!IsSourceBlock(blockType) ) return MazeBlockType.NULL;
		foreach( MazeBlockType val in mazeBlockTypeEnumArray )
		{
			if(SEnum.GetStringValue(val)  == SEnum.GetStringValue(blockType).ToUpper() ) return val;
		}
		return MazeBlockType.NULL;
	}
	
	public static MazeBlockType GetSourceForSinkBlock(MazeBlockType blockType) {
		if(!IsSinkBlock(blockType) ) return MazeBlockType.NULL;
		foreach( MazeBlockType val in mazeBlockTypeEnumArray )
		{
			if(SEnum.GetStringValue(val)  == SEnum.GetStringValue(blockType).ToLower() ) return val;
		}
		return MazeBlockType.NULL;
	}

	static Array mazeBlockTypeEnumArray = Enum.GetValues(typeof(MazeBlockType));
	public static MazeBlockType StringToBlockType(string blockType) {
		foreach( MazeBlockType val in mazeBlockTypeEnumArray )
		{
			if(SEnum.GetStringValue(val) == blockType) return val;
			//Debug.LogError(String.Format("{0}: {1}", Enum.GetName(typeof(MazeBlockType), val) , SEnum.GetStringValue(val)  ) );
		}
		
		/*if ( BaseBlock.IsBlock(blockType) ) {
			return MazeBlockType.BLOCK1;
		} else if (blockType == SEnum.GetStringValue(MazeBlockType.FREE) ) {
			return MazeBlockType.FREE;
		}*/
		return MazeBlockType.FREE;
	}
	#endregion

}

public enum MazePathType {
	TYPE1,
	TYPE2,
	TYPE3,
	DEFAULT,
	NULL
}

public enum MazePathBlockType {
	SINK,
	SOURCE,
	BRIDGE,
	NULL
}

public enum MazeBlockType {
	[StringValue("X")]
	BLOCK1,
	BLOCK2,
	[StringValue("a")]
	SOURCE1,
	[StringValue("A")]
	SINK1,
	[StringValue("b")]
	SOURCE2,
	[StringValue("B")]
	SINK2,
	[StringValue(".")]
	FREE,
	END,
	NULL
}

/*:hint
public class LogCategory
{
	private LogCategory(string value) { Value = value; }
	
	public string Value { get; set; }
	
	public static LogCategory Trace { get { return new LogCategory("Trace"); } }
	public static LogCategory Debug { get { return new LogCategory("Debug"); } }
}
*/