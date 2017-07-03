using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Purchasing
{
	static class UnityIAPInstaller
	{
		static readonly string k_ServiceName = "IAP";
		static readonly string k_PackageName = "Unity IAP";
		static readonly string k_PackageFile = "Plugins/UnityPurchasing/UnityIAP.unitypackage";
		static readonly string k_InstallerFile = "Plugins/UnityPurchasing/Editor/UnityIAPInstaller.cs";
		static readonly string k_ObsoleteFilesCSVFile = "Plugins/UnityPurchasing/Editor/ObsoleteFilesOrDir.csv";
		static readonly string k_ObsoleteGUIDsCSVFile = "Plugins/UnityPurchasing/Editor/ObsoleteGUIDs.csv";
		static readonly string k_IAPHelpURL = "https://docs.unity3d.com/Manual/UnityIAPSettingUp.html";
		static readonly string k_ProjectHelpURL = "https://docs.unity3d.com/Manual/SettingUpProjectServices.html";
		static readonly string k_PrefsKey_ImportingAssetPackage = "UnityIAPInstaller_ImportingAssetPackage"; // Prevent multiple simultaneous installs
		static readonly string k_PrefsKey_LastAssetPackageImport = "UnityIAPInstaller_LastAssetPackageImportDateTimeBinary";
		static readonly double k_MaxLastImportReasonableTicks = 30 * 10000000; // Installs started n seconds from 'now' are not considered 'simultaneous'

		static readonly string[] k_ObsoleteFilesOrDirectories = GetFromCSV(GetAbsoluteFilePath(k_ObsoleteFilesCSVFile));
		static readonly string[] k_ObsoleteGUIDs = GetFromCSV(GetAbsoluteFilePath(k_ObsoleteGUIDsCSVFile));

		static readonly bool k_RunningInBatchMode = Environment.CommandLine.ToLower().Contains(" -batchmode");

	    private static Type GetPurchasing()
	    {
	        return (
	            from assembly in AppDomain.CurrentDomain.GetAssemblies()
	            from type in assembly.GetTypes()
	            where type.Name == "UnityPurchasing" && type.GetMethods().Any(m => m.Name == "Initialize")
	            select type).FirstOrDefault();
	    }

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		static readonly bool k_IsIAPSupported = true;
#else
		static readonly bool k_IsIAPSupported = false;
#endif

#if UNITY_5_5_OR_NEWER && false // Service window prevents this from working properly. Disabling for now.
		static readonly bool k_IsEditorSettingsSupported = true;
#else
		static readonly bool k_IsEditorSettingsSupported = false;
#endif

#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts]
#endif
		/// <summary>
		/// * Install may be called multiple times during the AssetDatabase.ImportPackage
		///   process. Detect this and avoid restarting installation.
		/// * Install may fail unexpectedly in the middle due to crash. Detect 
		///   this heuristically with a timestamp, deleting mutex for multiple
		///   install detector. 
		/// </summary>
		static void Install ()
		{
			// Detect and fix interrupted installation
			FixInterruptedInstall();

			// Detect multiple calls to this method and ignore
			if (PlayerPrefs.HasKey(k_PrefsKey_ImportingAssetPackage))
			{
				// Resubscribe to "I'm done installing" callback as it's lost
				// on each Reload.
				EditorApplication.delayCall += OnComplete;
				return;
			}
		   
			if (!DisplayInstallerDialog())
			{
				DisplayCanceledInstallerDialog();
				OnComplete();
				return;
			}

			string packageAsset = GetAssetPath(k_PackageFile);

			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("Preparing to install the {0} asset package...", k_PackageName);
			}

			if (CanInstall(packageAsset))
			{
				// Record fact installation has started
				PlayerPrefs.SetInt(k_PrefsKey_ImportingAssetPackage, 1);
				// Record time installation started
				PlayerPrefs.SetString(k_PrefsKey_LastAssetPackageImport, DateTime.UtcNow.ToBinary().ToString());
				// Start async ImportPackage operation, causing one or more
				// Domain Reloads as a side-effect
				AssetDatabase.ImportPackage(packageAsset, false); 
				// All in-memory values hereafter may be cleared due to Domain
				// Reloads by async ImportPackage operation
				EditorApplication.delayCall += OnComplete;
			}
			else
			{
				OnComplete();
			}
		}

		/// <summary>
		/// Determines if can install the specified packageAsset.
		/// </summary>
		/// <returns><c>true</c> if can install the specified packageAsset; otherwise, <c>false</c>.</returns>
		/// <param name="packageAsset">Package asset.</param>
		static bool CanInstall(string packageAsset)
		{
			return k_IsIAPSupported && AssetExists(packageAsset) &&
				(GetPurchasing() != null || EnableServices()) &&
				DeleteObsoleteAssets(k_ObsoleteFilesOrDirectories, k_ObsoleteGUIDs);
		}

		/// <summary>
		/// Detects and fixes the interrupted install.
		/// </summary>
		static void FixInterruptedInstall()
		{
			if (PlayerPrefs.HasKey(k_PrefsKey_LastAssetPackageImport))
			{
				string lastImportDateTimeBinary = PlayerPrefs.GetString(k_PrefsKey_LastAssetPackageImport);

				long lastImportLong = 0;
				try {  
					lastImportLong = Convert.ToInt64(lastImportDateTimeBinary);
				} catch (SystemException e) {
					// Ignoring exception converting long
					// By default '0' value will trigger install-cleanup
				}

				DateTime lastImport = DateTime.FromBinary(lastImportLong);
				double dt = Math.Abs(DateTime.UtcNow.Ticks - lastImport.Ticks); 

				if (dt > k_MaxLastImportReasonableTicks)
				{
					Debug.Log("Detected interrupted installation, " + dt / 10000000 + " seconds ago. Reenabling install.");
					// Fix it!
					PlayerPrefs.DeleteKey(k_PrefsKey_ImportingAssetPackage);
					PlayerPrefs.DeleteKey(k_PrefsKey_LastAssetPackageImport);
				}
				else
				{
					// dt is not too large, installation okay to proceed
				}
			}
		}

		static void OnComplete ()
		{
			if (PlayerPrefs.HasKey(k_PrefsKey_ImportingAssetPackage))
			{
				// Cleanup mutexes for next install
				PlayerPrefs.DeleteKey(k_PrefsKey_ImportingAssetPackage);
				PlayerPrefs.DeleteKey(k_PrefsKey_LastAssetPackageImport);

				if (k_RunningInBatchMode)
				{
					Debug.LogFormat("Successfully imported the {0} asset package.", k_PackageName);
				}
			}

			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("Deleting {0} package installer files...", k_PackageName);
			}

			AssetDatabase.DeleteAsset(GetAssetPath(k_PackageFile));
			AssetDatabase.DeleteAsset(GetAssetPath(k_InstallerFile));
			AssetDatabase.DeleteAsset(GetAssetPath(k_ObsoleteFilesCSVFile));
			AssetDatabase.DeleteAsset(GetAssetPath(k_ObsoleteGUIDsCSVFile));

			AssetDatabase.Refresh();
			SaveAssets();

			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("{0} asset package install complete.", k_PackageName);
				EditorApplication.Exit(0);
			}
		}

		static bool EnableServices ()
		{
			if (!k_IsEditorSettingsSupported)
			{
				if (!DisplayEnableServiceManuallyDialog())
				{
					Application.OpenURL(k_IAPHelpURL);
				}

				return false;
			}

			if (string.IsNullOrEmpty(PlayerSettings.cloudProjectId))
			{
				if (!DisplayProjectConfigDialog())
				{
					Application.OpenURL(k_ProjectHelpURL);
				}

				return false;
			}

			if (DisplayEnableServiceDialog())
			{
#if UNITY_5_5_OR_NEWER
				Analytics.AnalyticsSettings.enabled = true;
				PurchasingSettings.enabled = true;
#endif

				SaveAssets();
				return true;
			}

			if (!DisplayCanceledEnableServiceDialog())
			{
				Application.OpenURL(k_IAPHelpURL);
			}

			return false;
		}

		static bool DisplayInstallerDialog ()
		{
			if (k_RunningInBatchMode) return true;

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_PackageName + " installer will determine if your project is configured properly " +
				"before importing the " + k_PackageName + " asset package.\n\n" +
				"Would you like to run the " + k_PackageName + " installer now?",
				"Install Now",
				"Cancel"
			);
		}

		static bool DisplayCanceledInstallerDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("User declined to run the {0} installer. Canceling installer process now...", k_PackageName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_PackageName + " installer has been canceled. " +
				"Please import the " + k_PackageName + " asset package again to continue the install.",
				"OK"
			);
		}

		static bool DisplayProjectConfigDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.Log("Unity Project ID is not currently set. Canceling installer process now...");
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"A Unity Project ID is not currently configured for this project.\n\n" +
				"Before the " + k_ServiceName + " service can be enabled, a Unity Project ID must first be " +
				"linked to this project. Once linked, please import the " + k_PackageName + " asset package again" +
				"to continue the install.\n\n" +
				"Select 'Help...' to see further instructions.",
				"OK",
				"Help..."
			);
		}

		static bool DisplayEnableServiceDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("The {0} service is currently disabled. Enabling the {0} Service now...", k_ServiceName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_ServiceName + " service is currently disabled.\n\n" +
				"To avoid encountering errors when importing the " + k_PackageName + " asset package, " +
				"the " + k_ServiceName + " service must be enabled first before importing the latest " + 
				k_PackageName + " asset package.\n\n" +
				"Would you like to enable the " + k_ServiceName + " service now?",
				"Enable Now",
				"Cancel"
			);
		}

		static bool DisplayEnableServiceManuallyDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("The {0} service is currently disabled. Canceling installer process now...", k_ServiceName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_ServiceName + " service is currently disabled.\n\n" +
				"Canceling the install process now to avoid encountering errors when importing the " + 
				k_PackageName + " asset package. The " + k_ServiceName + " service must be enabled first " +
				"before importing the latest " + k_PackageName + " asset package.\n\n" +
				"Please enable the " + k_ServiceName + " service through the Services window. " +
				"Then import the " + k_PackageName + " asset package again to continue the install.\n\n" +
				"Select 'Help...' to see further instructions.",
				"OK",
				"Help..."
			);
		}

		static bool DisplayCanceledEnableServiceDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("User declined to enable the {0} service. Canceling installer process now...", k_ServiceName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_PackageName + " installer has been canceled.\n\n" +
				"Please enable the " + k_ServiceName + " service through the Services window. " +
				"Then import the " + k_PackageName + " asset package again to continue the install.\n\n" +
				"Select 'Help...' to see further instructions.",
				"OK",
				"Help..."
			);
		}

		static bool DisplayDeleteAssetsDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("Found obsolete {0} assets. Deleting obsolete assets now...", k_PackageName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"Found obsolete assets from an older version of the " + k_PackageName + " asset package.\n\n" +
				"Would you like to remove these obsolete " + k_PackageName + " assets now?",
				"Delete Now",
				"Cancel"
			);
		}

		static bool DisplayCanceledDeleteAssetsDialog ()
		{
			if (k_RunningInBatchMode)
			{
				Debug.LogFormat("User declined to remove obsolete {0} assets. Canceling installer process now...", k_PackageName);
				return true;
			}

			return EditorUtility.DisplayDialog(
				k_PackageName + " Installer",
				"The " + k_PackageName + " installer has been canceled.\n\n" +
				"Please delete any previously imported " + k_PackageName + " assets from your project. " +
				"Then import the " + k_PackageName + " asset package again to continue the install.",
				"OK"
			);
		}

		static string GetAssetPath (string path)
		{
			return string.Concat("Assets/", path);
		}

		static string GetAbsoluteFilePath (string path)
		{
			return Path.Combine(Application.dataPath, path.Replace('/', Path.DirectorySeparatorChar));
		}

		static string[] GetFromCSV (string filePath)
		{
			var lines = new List<string>();
			int row = 0;

			if (File.Exists(filePath))
			{
				try
				{
					using (var reader = new StreamReader(filePath))
					{
						while (!reader.EndOfStream)
						{
							string[] line = reader.ReadLine().Split(',');
							lines.Add(line[0].Trim().Trim('"'));
							row++;
						}
					}
				}
				catch (Exception e) 
				{
					Debug.LogException(e);
				}
			}

			return lines.ToArray();
		}

		static bool AssetExists (string path)
		{
			if (path.Length > 7)
				path = path.Substring(7);
			else return false;

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				path = path.Replace("/", @"\");
			}

			path = Path.Combine(Application.dataPath, path);

			return File.Exists(path) || Directory.Exists(path);
		}

		static bool AssetsExist (string[] legacyAssetPaths, string[] legacyAssetGUIDs, out string[] existingAssetPaths)
		{
			var paths = new List<string>();

			for (int i = 0; i < legacyAssetPaths.Length; i++)
			{
				if (AssetExists(legacyAssetPaths[i]))
				{
					paths.Add(legacyAssetPaths[i]);
				}
			}

			for (int i = 0; i < legacyAssetGUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(legacyAssetGUIDs[i]);

				if (AssetExists(path) && !paths.Contains(path))
				{
					paths.Add(path);
				}
			}

			existingAssetPaths = paths.ToArray();

			return paths.Count > 0;
		}

		static bool DeleteObsoleteAssets (string[] paths, string[] guids)
		{
			var assets = new string[0];

			if (!AssetsExist(paths, guids, out assets)) return true;

			if (DisplayDeleteAssetsDialog())
			{
				for (int i = 0; i < assets.Length; i++)
				{
					FileUtil.DeleteFileOrDirectory(assets[i]);
				}

				AssetDatabase.Refresh();
				SaveAssets();
				return true;
			}

			DisplayCanceledDeleteAssetsDialog();
			return false;
		}

		/// <summary>
		/// Solves issues seen in projects when deleting other files in projects
		/// after installation but before project is closed and reopened.
		/// Script continue to live as compiled entities but are not stored in 
		/// the AssetDatabase. 
		/// </summary>
		static void SaveAssets ()
		{
#if UNITY_5_5_OR_NEWER
			AssetDatabase.SaveAssets(); // Not reliable prior to major refactoring in Unity 5.5.
#else
			EditorApplication.SaveAssets(); // Reliable, but removed in Unity 5.5.
#endif
		}
	}
}
