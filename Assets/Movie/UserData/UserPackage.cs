
using System.Collections.Generic;
using System;

[System.Serializable]

public class UserPackage {
	public string package_id {get; set;}
    //it is package id basically
    //public string package_id

    public string status {get; set;}

    //if played
    public int solved {get;set;}
    public int score {get;set;}

    //if time unlock
    //public long unlockStartTime {get;set;}
    public long TimeUnlockEndTime {get;set;}

    //public LockStatus lockStatus;
    static Array lockStatusEnumArray = Enum.GetValues(typeof(LockStatus));
    public static LockStatus StringToLockStatus(string lockStatus) {
        foreach( LockStatus val in lockStatusEnumArray )
        {
            if( Utility.StringEquals( SEnum.GetStringValue(val), lockStatus) ) return val;
            //Debug.LogError(String.Format("{0}: {1}", Enum.GetName(typeof(MazeBlockType), val) , SEnum.GetStringValue(val)  ) );
        }
        return LockStatus.INVISIBLE;
    }

    public LockStatus lockStatus {
        get {
            return StringToLockStatus(status);
        }
        set {
            status = SEnum.GetStringValue(value);
        }
    }


    public UserPackage(string id, LockStatus lockStatus ) {
        this.lockStatus = lockStatus;
        this.package_id = id;
    }


}

public enum LockStatus {
    [StringValue("INVISIBLE")]
    INVISIBLE,
    [StringValue("CURRENCY_LEVEL_LOCK")]
    CURRENCY_LEVEL_LOCK , //: will not be saved in userpackage : cost will be factor of diff with user level
    [StringValue("FORCED_LEVEL_LOCK")]
    FORCED_LEVEL_LOCK, // : will not be saved in userpackage : " ,
    [StringValue("CURRENCY_TIME_LOCK")]
    CURRENCY_TIME_LOCK, // : initiated for user
    [StringValue("JUST_UNLOCKED")]
    JUST_UNLOCKED, //
    [StringValue("UNPLAYED")]
    UNPLAYED, //
    [StringValue("PLAYED")]
    PLAYED, //     : userpackage contain score detail
    [StringValue("WELLPLAYED")]
    WELLPLAYED, // : "
    [StringValue("COMPLETED")]
    COMPLETED,
    [StringValue("EXPIRED")]
    EXPIRED
}