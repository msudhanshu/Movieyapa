	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using KiwiCommonDatabase;
	using SimpleSQL;
	using System.Reflection;
	using System.Linq;

	partial class DatabaseManager : Manager<DatabaseManager>
	{
		public SimpleSQLManager _dbManager;
		private static IBaseDbHelper dbHelper = null;
		private static DatabaseManager _instance;
		public static string[] DB_TABLES = new string[]{"MarketVersion", "LevelSceneData"};

			override public void StartInit ()
			{
					Debug.Log ("For iOS: Setting Application.targetFrameRate = 60");
					Application.targetFrameRate = 60;

					if (dbHelper == null) {
							CachedSimpleSqlDbHelper ssDbHelper = CachedSimpleSqlDbHelper.GetInstance ();
							ssDbHelper.SetSimpleSqlManager (_dbManager);
							dbHelper = (IBaseDbHelper)ssDbHelper;	

							if (!_dbManager) {
									Debug.LogError ("You need to have an SimpleSQLManager set up from Unity.");
							}
					}
					
					//testAssetData();
					createSchema ();
	//				updateSchema ();
					//testAllDbOps ();
					transform.parent.gameObject.BroadcastMessage ("DependencyCompleted", ManagerDependency.DATABASE_INITIALIZED);


	//				AssetState state1 = DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<AssetState>(37);
	//				
	//				KDbQuery<AssetState> dbQuery = new KDbQuery<AssetState>(new DbOpEq("nextId", 38));
	//				AssetState state2 = DatabaseManager.GetInstance ().GetDbHelper ().QueryForFirst<AssetState> (dbQuery);
	//				
	//				Debug.Log("AssetState(37) -- obj1 = " + state1.id + ", obj2 = " + state2.id);
	//				Debug.Log ("Equals? ==  " + state1.Equals(state2));
	//				Debug.Log ("Equals? ==  " + state2.Equals(state1));
	//				Debug.Log ("==? ==  " + (state1 == state2));
	//				Debug.Log ("==? ==  " + (state2==state1));

			}

			override public void PopulateDependencies ()
			{
			}

			public static DatabaseManager GetInstance ()
			{
					if (!_instance) {
							_instance = (DatabaseManager)FindObjectOfType (typeof(DatabaseManager));
							if (!_instance) {
									Debug.LogError ("You need to have an object of Database Manager");
							}
					}
			
					return _instance;
			}


		public IBaseDbHelper GetDbHelper ()
		{
			return dbHelper;
		}

		/**
		 * Test methods for database operations
		 **/
		public void testExecuteSql ()
		{

		}
		
		public void testCreateTable ()
		{
			dbHelper.CreateTableIfNotExists<SpecialStarShip> ();
		}
		
		public void testQuery ()
		{
			string sql = "SELECT * FROM SpecialStarShip";
			List<SpecialStarShip> starships = dbHelper.Query<SpecialStarShip> (sql);
			foreach (SpecialStarShip starship in starships) {
				Debug.Log ("StarShip: ID=" + starship.id + ", Name=" + starship.StarShipName + ", HomePlanet=" + starship.HomePlanet);
			}
		}
		
		public void testInsert ()
		{
	//		string suffix = "" + Random.Range (1, 1000);
	//		SpecialStarShip ship = new SpecialStarShip{StarShipName = ("SS-Name-G"+suffix), HomePlanet = ("HP-G"+suffix), Range = 0.0f, Armor = 0.01f, Firepower = 0.02f};
	//		dbHelper.Insert<SpecialStarShip> (ship);

	//		string sql;
	//		sql = "INSERT INTO SpecialStarShip " +
	//			"(StarShipName, HomePlanet) " +
	//				"VALUES (?, ?)";
	//		dbManager.Execute(sql, "SS-Name-A", "HP-A");
		}
		
		public void testDelete ()
		{

		}
		
		public void testUpdate ()
		{
			string sql = "SELECT * FROM SpecialStarShip where id = ?";
			List<SpecialStarShip> starships = dbHelper.Query<SpecialStarShip> (sql, 7);
			foreach (SpecialStarShip starship in starships) {
				Debug.Log ("StarShip: ID=" + starship.id + ", Name=" + starship.StarShipName + ", HomePlanet=" + starship.HomePlanet);
				starship.StarShipName = starship.StarShipName + "-2";
				dbHelper.Update<SpecialStarShip> (starship);
			}
		}

		public void testAssetData (List<AssetData> assetsData, int marketVersion)
		{
			if (assetsData != null) {
				foreach (AssetData assetData in assetsData) {
					dbHelper.InsertOrUpdate<AssetData> (assetData);
				}
			}
			List<AssetData> dbAssets = dbHelper.QueryForAll<AssetData> ();
			foreach (AssetData assetData in dbAssets) {
				Debug.Log ("AssetData found for " + assetData.name);
			}
			UpdateMarketVersion (marketVersion);
		}

		public void InsertMarketTable<T> (List<T> data)
		{
			if (ServerConfig.DB_PACKING)
				dbHelper.UpdateTableSchema<T>();

			if (data != null) {
				foreach (T assetData in data) {
					dbHelper.InsertOrUpdate<T> (assetData);
				}
			}
		}

		public void UpdateMarketVersion(int marketVersion) {
			MarketVersion dbVersion = dbHelper.QueryObjectById<MarketVersion> (1);
			if (dbVersion == null) {
				dbVersion = new MarketVersion ();
				dbVersion.id = 1;
				dbVersion.version = marketVersion;
				dbHelper.Insert<MarketVersion> (dbVersion);
			} else {
				dbVersion.version = marketVersion;
				dbHelper.Update<MarketVersion> (dbVersion);
			}
		}


		public void testAllDbOps ()
		{
			SpecialWeapon wp = new SpecialWeapon ();
			wp.id = 1;
			wp.WeaponName = "wp-a";
			wp.WeaponTypeDescription = "wp-desc-a";
			dbHelper.Insert<SpecialWeapon> (wp);

			Debug.Log ("specialweapon = " + wp.ToString());
	//
	//		SpecialWeapon wp1 = new SpecialWeapon ();
	//		wp1.id = 1;
	//		wp1.WeaponName = "wp-a";
	//		wp1.WeaponTypeDescription = "wp-desc-b";
	//
	//		dbHelper.Update<SpecialWeapon> (wp1);

			SpecialStarShip ssShip = new SpecialStarShip ();
			ssShip.id = 1;
			ssShip.StarShipName = "Ref-SSShip-Name-A";
			ssShip.HomePlanet = "Ref-SSShip-Planet-A";
			ssShip.specialWeaponId = wp.id;
			dbHelper.Insert<SpecialStarShip>(ssShip);

			Debug.Log ("RelationCheck -- specialweapon = " + wp.ToString());
			Debug.Log("RelationCheck --  ssShip = " + ssShip.ToString());

			/**

			PendingDownload rpd = new PendingDownload ();

			rpd.GetType ().GetProperty ("bundleName", BindingFlags.Public | BindingFlags.Instance).SetValue (rpd, "reflection-bundle-a", null);
			Debug.Log ("Pending download on setting with reflection = " + rpd);

	//		SeedData.SeedPendingDownloads ();
			Debug.Log ("QUERY ALL ITEMS ---- ");
			dbHelper.QueryForAll<SpecialStarShip> ();
			testQuery ();

			KDbQuery<PendingDownload> dbquery = new KDbQuery<PendingDownload>(new DbOpEq("bundleName", "model_1"));
			List<PendingDownload> pds = dbHelper.QueryForAll<PendingDownload> (dbquery);
			foreach(PendingDownload pd in pds){
				Debug.Log("PendingDownload: (model_1) -- " + pd.ToString());
			}

			dbquery = new KDbQuery<PendingDownload>(new DbOpGt("priority", 3));
			pds = dbHelper.QueryForAll<PendingDownload> (dbquery);
			foreach(PendingDownload pd in pds){
				Debug.Log("PendingDownload: (priority > 3) -- " + pd.ToString());
			}

			dbquery = new KDbQuery<PendingDownload>(new BaseDbOp[]{new DbOpEq("priority", 2), new DbOpEq("bundleName", "model_3")});
			pds = dbHelper.QueryForAll<PendingDownload> (dbquery);
			foreach(PendingDownload pd in pds){
				Debug.Log("PendingDownload: (model_3, priority=2) -- " + pd.ToString());
			}

			dbquery = new KDbQuery<PendingDownload>(new DbOpIn("priority", new ArrayList(){2,3}));
			pds = dbHelper.QueryForAll<PendingDownload> (dbquery);
			foreach(PendingDownload pd in pds){
				Debug.Log("PendingDownload: (priority=2,3) -- " + pd.ToString());
			}

			dbquery = new KDbQuery<PendingDownload>( 
			                        new DbOpOr(
										new DbOpAnd(
											new DbOpEq("status", 2), new DbOpEq("priority", 3)
			            				), 
										new DbOpAnd(
											new DbOpEq("status", 1), new DbOpEq("priority", 5)
										),
										new DbOpAnd(
											new DbOpEq("status", 2), new DbOpEq("priority", 2)
										)
									));
			KDbQueryAttribute dbqueryAttribute = new KDbQueryAttribute ();
			dbqueryAttribute.addOrderByColumn ("status", false);
			dbqueryAttribute.addOrderByColumn ("bundleName", true);
			dbqueryAttribute.addSelectedColumn ("bundleName");
			dbqueryAttribute.addSelectedColumn ("status");
			dbqueryAttribute.setOffset (1);
			dbqueryAttribute.setLimit (2);
			dbquery.setQueryAttribute (dbqueryAttribute);
			pds = dbHelper.QueryForAll<PendingDownload> (dbquery);
			foreach(PendingDownload pd in pds){
				Debug.Log("PendingDownload: (s,p=2,3 OR 1,4) -- " + pd.ToString());
			}

			int randomInt = Random.Range (1, 1000);
			string suffix = "" + randomInt;

			Debug.Log ("INSERT ITEM with suffix: " + suffix);
			SpecialStarShip ship = new SpecialStarShip{StarShipName = ("SS-Name-"+suffix), HomePlanet = ("HP-"+suffix), Range = 0.0f, Armor = 0.01f, Firepower = 0.02f};
			dbHelper.Insert<SpecialStarShip> (ship);

			string searchShipName = "SS-Name-" + suffix;
			Debug.Log ("TO QUERY ITEM with name BY queryObjectById: " + ship.id);

			SpecialStarShip ship1 = dbHelper.QueryObjectById<SpecialStarShip> (ship.id);
			Debug.Log ("Result of queryObjectById -- StarShip: ID=" + ship1.id + ", Name=" + ship1.StarShipName + ", HomePlanet=" + ship1.HomePlanet);

			string sql = "SELECT * FROM SpecialStarShip where StarShipName = ?";
			List<SpecialStarShip> starships = dbHelper.Query<SpecialStarShip> (sql, searchShipName);
			foreach (SpecialStarShip starship in starships) {
				Debug.Log ("StarShip: ID=" + starship.id + ", Name=" + starship.StarShipName + ", HomePlanet=" + starship.HomePlanet);

				randomInt = Random.Range (1000, 2000);
				Debug.Log ("Update item from suffix: " + suffix + "  - TO - " + randomInt);
				starship.StarShipName = "SS-Name-" + randomInt;
				dbHelper.Update<SpecialStarShip> (starship);
			}

			Debug.Log ("QUERY ALL ITEMS AFTER UPDATE");
			testQuery ();

			string deleteItemName = "SS-Name-" + randomInt;
			Debug.Log ("DELETE ITEM with StarShipName= " + deleteItemName);
			sql = "SELECT * FROM SpecialStarShip where StarShipName = ?";
			List<SpecialStarShip> starships_2 = dbHelper.Query<SpecialStarShip> (sql, deleteItemName);
			foreach (SpecialStarShip starship in starships_2) {
				Debug.Log ("DELETE StarShip: ID=" + starship.id + ", Name=" + starship.StarShipName + ", HomePlanet=" + starship.HomePlanet);
				dbHelper.Delete<SpecialStarShip> (starship);
			}

			Debug.Log ("QUERY ALL ITEMS AFTER DELETE");
			testQuery ();

	**/
		}


		public void createSchema ()
		{
			foreach (string typeName in DB_TABLES) {
				MethodInfo method = getGenericHelperMethod (typeName, "CreateTableIfNotExists");
				if (method != null)
					method.Invoke (dbHelper, new object[]{});
			}
			
		}
		
		private MethodInfo getGenericHelperMethod (string typeName, string methodName)
		{
			System.Type dtype = System.Type.GetType (typeName);
			System.Type dbInterface = dbHelper.GetType ().GetInterfaces ().Single (e => e.Name == "IBaseDbHelper");
			MethodInfo method = null;
			if (dtype != null && dbInterface != null) {
				method = dbInterface.GetMethod (methodName).MakeGenericMethod (new System.Type[] { dtype });
			}
			return method; 
			
		}
		
		public void updateSchema ()
		{
			foreach (string typeName in DB_TABLES) {
				MethodInfo method = getGenericHelperMethod (typeName, "UpdateTable");
				if (method != null)
					method.Invoke (dbHelper, new object[]{});
			}
		}

	}

