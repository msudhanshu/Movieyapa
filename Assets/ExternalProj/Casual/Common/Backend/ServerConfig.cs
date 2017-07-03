using System;

public class ServerConfig {
	public static String BASE_URL = "http://localhost:9000/movie/"; // "http://52.25.129.93";
	//public static String FIREBASE_CLOUD_FUNCTION_BASE_URL = "https://us-central1-unityfirebasetest-5c0bc.cloudfunctions.net";
	public static String BATCH_REQUEST_URL = BASE_URL + "/batch/process?";
	public static char[] commaDelimiters = {','};
	public static char[] hyphenDelimiters = {'-'};
	public static char[] colonDelimiters = {':'};
	public static bool BATCHING_ENABLED = false;
	public static bool SERVER_ENABLED = true ;
	public static bool DB_PACKING = true;
	public static long USER_ASSETS_OFFSET = 1000000;
	public static bool LOADING_SERVER_DATA = false;
	public static long serverTimeAtSessionStart = 0;
	public static long localTimeAtSessionStart = 0;
	public static int MAX_INTEGER_VALUE = 1000000;
//
//	public static string DiffUrl {
//		get { 
//			if (Config.FIREBASE_SERVER) {
//				return FIREBASE_CLOUD_FUNCTION_BASE_URL+"/diff?user_id=<user_id>&version=<version>";
//			} else {
//				return "http://qa6.kiwiup.com/g48/diff?user_id=<user_id>&version=<version>";
//			}
//		}
//	}

}

