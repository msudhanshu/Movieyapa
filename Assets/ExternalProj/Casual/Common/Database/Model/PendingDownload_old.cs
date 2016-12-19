using UnityEngine;
using System;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

public class PendingDownload_old: BaseDbModel
{

	public PendingDownload_old ()
		{

		}
	public PendingDownload_old (string bundleName, int priority, int status)
		{
				this.bundleName = bundleName;
				this.priority = priority;
				this.status = status;
		}

		[PrimaryKey, MaxLength(60), NotNull]
		public string bundleName { get; set; }

		[Indexed, Default(1)]
		public int priority { get; set; }

		[Indexed, Default(0)]
		public int status{ get; set; }

		public static void Add (string bundlename, int priority, int status)
		{
				PendingDownload pendingDownload = new PendingDownload (bundlename, priority, status);
				try {
						DatabaseManager.GetInstance ().GetDbHelper ().Insert<PendingDownload> (pendingDownload);
						Debug.Log ("Added successfully " + bundlename);
			
				} catch (Exception e) {
						Debug.LogWarning ("Unable to insert " + bundlename + " " + e.Message);
				}
		}
		
		public override string ToString(){
			return "PendingDownload: bundleName=" + bundleName + ", priority=" + priority + ", status=" + status;
		}
}

