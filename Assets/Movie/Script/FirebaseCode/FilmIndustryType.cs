using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using System.Text;
using System.Threading.Tasks;


public enum FilmTypeEnum {
	BOLLYWOOD,
	HOLLYWOOD,
	TOLLYWOOD,
	BHOJPURI,
	NONE
}

public class FilmIndustryType {
	private static string ACTIVE_FILM_PREF_TAG = "active_film_type";
	private static FilmTypeEnum Active_Film_Type_Cache = FilmTypeEnum.NONE;

	public static void SetActiveFilmType( string filmType) {
		FilmTypeEnum f = StringToFilmType (filmType);
		if (f != FilmTypeEnum.NONE) {
			PlayerPrefs.SetString (ACTIVE_FILM_PREF_TAG, filmType);
			Active_Film_Type_Cache = f;
		} else {
			Debug.LogError ("FilmIndustryType : trying to set non-existing filmtype : " + filmType);
		}
	}

	public static void SetActiveFilmType( FilmTypeEnum filmType) {
		PlayerPrefs.SetString (ACTIVE_FILM_PREF_TAG, FilmTypeToString(filmType));
		Active_Film_Type_Cache = filmType;
	}

	public static string GetActiveFilmTypeName() {
		return FilmTypeToString (GetActiveFilmType ());
	}

	public static FilmTypeEnum GetActiveFilmType() {
		if (Active_Film_Type_Cache == FilmTypeEnum.NONE) {
			string filmtypestring = PlayerPrefs.GetString (ACTIVE_FILM_PREF_TAG, FilmTypeToString (FilmTypeEnum.NONE));
			Active_Film_Type_Cache =  StringToFilmType (filmtypestring);
			return Active_Film_Type_Cache;
		}
		return FilmTypeEnum.NONE;
	}

	public static bool IsFilmTypeSet() {
		return false;
		return GetActiveFilmType() != FilmTypeEnum.NONE;
	}

	public static string FilmTypeToString(FilmTypeEnum filmType) {
		return filmType.ToString ();
	}

	public static FilmTypeEnum StringToFilmType(string filmTypeString) {
		switch(filmTypeString) {
		case "BOLLYWOOD":
			return FilmTypeEnum.BOLLYWOOD;
			break;
		case "HOLLYWOOD":
			return FilmTypeEnum.HOLLYWOOD;
			break;
		case "TOLLYWOOD":
			return FilmTypeEnum.TOLLYWOOD;
			break;
		case "BHOJPURI":
			return FilmTypeEnum.BHOJPURI;
			break;
		default:
			return FilmTypeEnum.NONE;
		}
	}

}
