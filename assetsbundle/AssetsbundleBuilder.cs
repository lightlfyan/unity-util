using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;


public class Builder : Editor
{

	// 预设存储的目录
	public static string sourcePath = Application.dataPath + "/prefabs";
	const string AssetBundlesOutputPath = "Assets/StreamingAssets";

	[MenuItem ("Tools/AssetBundle/Build-IOS")]
	public static void BuildAssetBundle ()
	{
		ClearAssetBundlesName ();
		SetAssetBundleName (sourcePath);

		//string outputPath = Path.Combine (AssetBundlesOutputPath,Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
		string outputPath = Path.Combine (AssetBundlesOutputPath, "AssetsBundle");
		if (!Directory.Exists (outputPath)) {
			Directory.CreateDirectory (outputPath);
		}

//		BuildPipeline.BuildAssetBundles (outputPath, 0, EditorUserBuildSettings.activeBuildTarget);
		BuildPipeline.BuildAssetBundles (outputPath, 0, BuildTarget.StandaloneOSXIntel64);
		AssetDatabase.Refresh ();

		Debug.Log ("打包完成");
	}

	static void ClearAssetBundlesName ()
	{
		int length = AssetDatabase.GetAllAssetBundleNames ().Length;
		Debug.Log (length);
		string[] oldAssetBundleNames = new string[length];
		for (int i = 0; i < length; i++) {
			oldAssetBundleNames [i] = AssetDatabase.GetAllAssetBundleNames () [i];
		}

		for (int j = 0; j < oldAssetBundleNames.Length; j++) {
			AssetDatabase.RemoveAssetBundleName (oldAssetBundleNames [j], true);
		}
		length = AssetDatabase.GetAllAssetBundleNames ().Length;
		Debug.Log (length);
	}

	static void SetAssetBundleName (string source)
	{
		DirectoryInfo folder = new DirectoryInfo (source);
		FileSystemInfo[] files = folder.GetFileSystemInfos ();
		int length = files.Length;
		for (int i = 0; i < length; i++) {
			if (files [i] is DirectoryInfo) {
				SetAssetBundleName (files [i].FullName);
			} else {
				if (!files [i].Name.EndsWith (".meta")) {
					_SetAssetBundleName (files [i].FullName);
				}
			}
		}
	}

	static void _SetAssetBundleName (string source)
	{
		string _source = Replace (source);
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);
		string _assetPath2 = _source.Substring (Application.dataPath.Length + 1);

		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
		string assetName = _assetPath2.Substring (_assetPath2.IndexOf ("/") + 1);
		assetName = assetName.Replace (Path.GetExtension (assetName), ".unity3d");
		Debug.Log (_assetPath2 + " " + assetName);
		assetImporter.assetBundleName = assetName;


		var deps = AssetDatabase.GetDependencies (_assetPath);
		foreach (var dep in deps) {
			if (dep.IndexOf (_assetPath) >= 0) {
				continue;
			}
//			Debug.Log (Path.GetExtension (dep));
			var guid = AssetDatabase.AssetPathToGUID (dep);
			Debug.Log ("dep: " + dep);

			AssetImporter assetImporterDep = AssetImporter.GetAtPath (dep);
			assetImporterDep.assetBundleName = guid;


		}
	}

	static string Replace (string s)
	{
		return s.Replace ("\\", "/");
	}
}

public class Platform
{
	public static string GetPlatformFolder (BuildTarget target)
	{
		switch (target) {
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "IOS";
		case BuildTarget.WebPlayer:
			return "WebPlayer";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
		default:
			return null;
		}
	}
}
