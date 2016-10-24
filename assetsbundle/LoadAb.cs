using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadAb : MonoBehaviour
{

	public List<AssetBundle> objs = new List<AssetBundle> ();

	public List<GameObject> models = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (load ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	IEnumerator load ()
	{
		string basepath = "file://" + Application.streamingAssetsPath + "/AssetsBundle/";
		WWW w = new WWW (basepath + "AssetsBundle");
		yield return w;
		if (w.error != null) {
			Debug.Log (w.error);
		}

		List<string> needload = new List<string> ();

		AssetBundleManifest info = w.assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		var alls = info.GetAllAssetBundles ();
		foreach (var name in alls) {
			var deps = info.GetAllDependencies (name);
			if (deps.Length <= 0) {
				needload.Add (name);
				continue;
			}
			foreach (var d in deps) {
				needload.Add (d);
			}

			needload.Add (name);
		}

		Dictionary<string, bool> loaded = new Dictionary<string, bool> ();
		foreach (var name in needload) {
			if (loaded.ContainsKey (name)) {
				continue;
			}

			w = new WWW (basepath + name);
			yield return w;

			Debug.Log (name);

			if (name.EndsWith (".unity3d")) {
				var objname = name.Replace (Path.GetExtension (name), "");
				var obj = w.assetBundle.LoadAsset<GameObject> (objname);
				models.Add (obj);
			} else {
//				var names = w.assetBundle.GetAllAssetNames ();
//				Debug.Log ("assetsname: " + names [0]);
				objs.Add (w.assetBundle);
			}

			loaded.Add (name, true);
		}

		foreach (var m in models) {
			GameObject.Instantiate (m);
		}
	}
}